using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChallengeAssistant.Interfaces;
using DevryBot.Options;
using DevryDomain.Models;
using DevryInfrastructure.Persistence;
using DisCatSharp.Entities;
using ImageCreator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UnofficialDevryIT.Architecture.Models;

namespace DevryBot.Services
{
    public class GamificationService : BackgroundService, IGamificationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IBot _bot;
        private readonly ChallengeOptions _options;
        private readonly ILogger<GamificationService> _logger;
        private readonly IImageService _imageService;
        private readonly IChallengeApi _api;

        private static readonly Random Random = new();
        
        private readonly string[] _mapping = new[]
        {
            ":zero:",
            ":one:",
            ":two:",
            ":three:",
            ":four:",
            ":five:",
            ":six:",
            ":seven:",
            ":eight:",
            ":nine:"
        };
        
        public GamificationService(IApplicationDbContext context, IBot bot, IOptions<ChallengeOptions> options, ILogger<GamificationService> logger, IImageService imageService, IChallengeApi api)
        {
            _context = context;
            _bot = bot;
            _logger = logger;
            _imageService = imageService;
            _api = api;
            _options = options.Value;
        }

        public async Task<ResultObject> CreateChallenge(Challenge challenge, string category, List<ChallengeResponse> responses)
        {
            if (await _context.Challenges.AnyAsync(x =>
                x.Title == challenge.Title &&
                x.Question == challenge.Question))
            {
                return ResultObject.Failure("Challenge with the same title and question already exists");
            }

            GamificationCategory gamificationCategory = await _context.GamificationCategories.FirstOrDefaultAsync(x=>x.Name == category);

            // Add the category if it does not exist
            if (gamificationCategory == null)
            {
                gamificationCategory = new GamificationCategory()
                {
                    Name = category
                };
                
                _context.GamificationCategories.Add(gamificationCategory);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Gamification Category with id: {gamificationCategory.Id}");
            }

            challenge.GamificationCategoryId = gamificationCategory.Id;

            _context.Challenges.Add(challenge);
            await _context.SaveChangesAsync();

            foreach (var response in responses)
            {
                response.ChallengeId = challenge.Id;
                _context.ChallengeResponses.Add(response);
            }

            await _context.SaveChangesAsync();
            
            return ResultObject.Success();
        }

        public async Task DeleteChallenge(ulong id)
        {
            var responses = await _context.ChallengeResponses.Where(x => x.ChallengeId == id).ToListAsync();

            foreach (var response in responses)
                _context.ChallengeResponses.Remove(response);

            var challenge = await _context.Challenges.FirstOrDefaultAsync(x => x.Id == id);

            if (challenge != null)
                _context.Challenges.Remove(challenge);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Challenge>> GetChallenges()
        {
            return await _context.Challenges
                .Include(x=>x.Responses)
                .ToListAsync();
        }

        public async Task<Challenge> GetDailyChallenge()
        {
            return await _context.Challenges
                .Include(x => x.Responses)
                .FirstOrDefaultAsync(x => x.IsActive);
        }

        public Task<ResultObject> PostChallenge()
        {
            throw new NotImplementedException();
        }

        public async Task ProcessChallenge()
        {
            var challenge = await GetDailyChallenge();

            if (challenge == null)
            {
                _logger.LogWarning($"No daily challenge to process");
                return;
            }
            
            challenge.IsActive = false;

            _context.Challenges.Update(challenge);
            await _context.SaveChangesAsync();

            DiscordMessage message = null;
            
            try
            {
                message = await _bot.MainGuild.Channels[_options.Channel]
                    .GetMessageAsync(challenge.DiscordMessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "\nUnable to process daily challenge... could not retrieve discord message", ex);
                return;
            }

            // Double checking this isn't null
            if (message == null)
            {
                _logger.LogError("Unable to process daily challenge... could not retrieve discord message");
                return;
            }

            DiscordChannel congratsChannel = _bot.MainGuild.Channels[_options.CongratsChannel];
            
            for (int i = 0; i < challenge.Responses.Count; i++)
            {
                if (i >= challenge.Responses.Count)
                    break;

                var response = challenge.Responses[i];

                if (!response.IsCorrect || response.Reward <= 0)
                    continue;

                IReadOnlyList<DiscordUser> reactions;

                try
                {
                    reactions = await message.GetReactionsAsync(DiscordEmoji.FromName(_bot.Client, _mapping[i]));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    continue;
                }
                
                foreach (DiscordUser user in reactions)
                {
                    var member = await _bot.MainGuild.GetMemberAsync(user.Id);

                    _logger.LogInformation(
                        $"Giving {response.Reward} to {member.DisplayName} for challenge {challenge.Id}");
                    
                    await AddReward(member.Id, challenge.GamificationCategoryId, response.Reward);
                }
                
                string url = await _imageService.RandomImageUrl("fireworks") ??
                             _options.Images[Random.Next(0, _options.Images.Length)];

                StringBuilder builder = new();
                builder.AppendLine("Congratulations, " + string.Join(", ", reactions.Select(x => x.Mention)));

                bool multiple = reactions.Count(x => !x.IsBot) > 1;

                if (response.IsCorrect)
                    if (multiple)
                        builder.AppendLine("You have all answered correctly!");
                    else
                        builder.AppendLine("You have answered correctly!");
                else
                {
                    if (multiple)
                        builder.AppendLine("Not quite, but you were all pretty darn close!");
                    else
                        builder.AppendLine("Not quite, but you were pretty darn close!");
                }
                
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                    .WithAuthor("Daily Challenge")
                    .WithTitle(challenge.Title)
                    .WithDescription(builder.ToString())
                    .AddField("Reward", response.Reward.ToString())
                    .WithTimestamp(DateTime.Today)
                    .WithImageUrl(url);
                
                
                await congratsChannel.SendMessageAsync(embed.Build());
                await Task.Delay(TimeSpan.FromSeconds(5));
            }


            // Now delete the challenge since we no longer need it
            foreach (var response in challenge.Responses)
                _context.ChallengeResponses.Remove(response);

            _context.Challenges.Remove(challenge);
            
            await _context.SaveChangesAsync();
        }

        async Task AddReward(ulong memberId, ulong categoryId, double amount)
        {
            GamificationEntry entry =
                await _context.GamificationEntries.FirstOrDefaultAsync(x => x.GamificationCategoryId == categoryId);

            if (entry == null)
            {
                entry = new GamificationEntry()
                {
                    GamificationCategoryId = categoryId,
                    UserId = memberId,
                    Value = amount
                };
                _context.GamificationEntries.Add(entry);
            }
            else
            {
                entry.Value += amount;
                _context.GamificationEntries.Update(entry);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var waitPeriod = TimeSpan.FromMinutes(1);
            
            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(waitPeriod);
            }
        }
    }
}
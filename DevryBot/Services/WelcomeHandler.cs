using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Options;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevryBot.Services
{
    public class WelcomeHandler : IWelcomeHandler
    {
        private readonly ILogger<WelcomeHandler> _logger;
        private readonly IOptions<WelcomeOptions> _options;
  
        /// <summary>
        /// Roles that we're expecting people to join for a given duration
        /// Perhaps a class lecture is happening right now
        ///
        /// Key: Class/Role to add user to
        /// Value: End Time for class
        /// </summary>
        public Dictionary<DiscordRole, DateTime> ClassExpectations = new();
        
        /// <summary>
        /// List of members that shall be welcomed in the current message queue
        /// </summary>
        private List<DiscordMember> welcomeQueue = new();

        /// <summary>
        /// When the current time exceeds trigger time -- the message will be triggered and the queue
        /// will be reset
        /// </summary>
        private DateTime triggerTime = DateTime.Now;

        private int messageInterval;
        private readonly CancellationToken _cancellationToken;
        private readonly DiscordChannel _welcomeChannel;
        private readonly ulong WELCOME_CHANNEL_ID = 618254766396538903;
        
        public WelcomeHandler(ILogger<WelcomeHandler> logger, IOptions<WelcomeOptions> options, IBot bot)
        {
            messageInterval = options?.Value.WelcomeMessageInterval ?? 30;
            _cancellationToken = default;
            _logger = logger;
            _options = options;
            _welcomeChannel = bot.MainGuild.Channels[WELCOME_CHANNEL_ID];
            Task.Factory.StartNew(ProcessQueue);
        }
        
        /// <summary>
        /// Based on the interaction ID format -- adds the associated role to <paramref name="member"/>
        /// </summary>
        /// <param name="member"></param>
        /// <param name="interactionId">{roleid}_{role}</param>
        public async Task AddRoleToMember(DiscordMember member, string interactionId)
        {
            // Must meet the following criteria
            if (!interactionId.EndsWith("_role"))
                return;
            
            ulong roleId = ulong.Parse(interactionId.Split("_").First());

            if (!_welcomeChannel.Guild.Roles.ContainsKey(roleId))
                return;

            await member.GrantRoleAsync(_welcomeChannel.Guild.Roles[roleId]);
        }
        
        /// <summary>
        /// Separate process that will handle welcoming users
        /// </summary>
        async Task ProcessQueue()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
                DateTime referenceTime = DateTime.Now;
                
                #region Remove Expired Class Expectations
                // Need to check and remove anything that is past its expiration time
                var remove = ClassExpectations
                    .Where(x => referenceTime > x.Value)
                    .Select(x => x.Key);
                
                if (remove.Any())
                {
                    _logger.LogInformation("The following classes exceeded their welcome-expiration-time:\n\t" +
                                                       $"{string.Join("\n\t", remove.Select(x=>x.Name))}");

                    foreach (var role in remove)
                        ClassExpectations.Remove(role);
                }
                #endregion
                
                // We will only send messages if the queue is > 0 AND if we have exceeded the trigger time 
                if (welcomeQueue.Count == 0)
                    continue;

                if (DateTime.Now < triggerTime)
                    continue;

                try
                {
                    string message = _options.Value.WelcomeMessage
                        .ToWelcomeMessage(welcomeQueue, _welcomeChannel);

                    _logger.LogInformation($"Welcoming {string.Join(", ", welcomeQueue.Select(x=>x.DisplayName))}, " +
                                                       $"following roles are appended to welcome message: \n\t" +
                                                       $"{string.Join("\n\t", ClassExpectations.Select(x=>x.Key.Name))}");
                    
                    // Reset queue for later
                    welcomeQueue.Clear();

                    DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                        .WithTitle("Welcome to Unofficial Devry IT")
                        .WithDescription(message);

                    DiscordMessageBuilder messageBuilder = new();
                    
                    var buttons = GenerateClassButtons();
                    
                    // Add some instruction to users to click the associated buttons -- if any
                    if (buttons.Count > 0)
                    {
                        embedBuilder.Description +=
                            "\n\nIf you're in one of the following majors or classes, please click the associated button(s)";

                        while (buttons.Count > 0)
                        {
                            var subsection = buttons.Take(5);
                            messageBuilder.AddComponents(subsection);
                            buttons.RemoveRange(0, subsection.Count());
                        }
                    }
                    
                    messageBuilder.AddEmbed(embedBuilder.Build());

                    await _welcomeChannel.SendMessageAsync(messageBuilder);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in WelcomeHandler while greeting users");
                }
            }
        }

        /// <summary>
        /// Generates <see cref="DiscordButtonComponent"/> for each class in
        /// <see cref="ClassExpectations"/>
        /// </summary>
        /// <returns>IEnumerable of <see cref="DiscordButtonComponent"/></returns>
        List<DiscordButtonComponent> GenerateClassButtons()
        {
            List<DiscordButtonComponent> buttons = new();

            int option = 0;
            foreach (var pair in ClassExpectations
                .OrderBy(x=>x.Key.Name)
                .Take(24))
            {
                DiscordButtonComponent component = new DiscordButtonComponent((ButtonStyle)(option + 1),
                    $"{pair.Key.Id}_role",
                    pair.Key.Name);
                
                buttons.Add(component);
                
                // Just trying to vary the colors that appear
                option++;
                option %= 4;
            }

            return buttons;
        }

        /// <summary>
        /// Add a class that we're expecting users to be coming from (Devry)
        /// Along with when the expiration time for when the button no longer
        /// gets appended to the welcome message
        /// </summary>
        /// <param name="role"><see cref="DiscordRole"/> that will be associated with a button</param>
        /// <param name="expirationTime"><see cref="DateTime"/> for when the button is removed from welcome message</param>
        public void AddClass(DiscordRole role, DateTime expirationTime)
        {
            if (ClassExpectations.ContainsKey(role))
                ClassExpectations[role] = expirationTime;
            else
                ClassExpectations.Add(role, expirationTime);
            
            _logger.LogInformation($"Expecting folks to be joining {role.Name} | {expirationTime.ToString("F")} | Appending Class Count {ClassExpectations.Count}");
        }
        
        /// <summary>
        /// Adds member to greeting queue
        /// </summary>
        /// <param name="member"></param>
        public void AddMember(DiscordMember member)
        {
            welcomeQueue.Add(member);

            // Based on the number of users we shall determine how much time gets added to our message interval
            // So the more users we have the less time is added thus -- message will occur sooner

            int interval = (int) (messageInterval - (0.1 * welcomeQueue.Count * messageInterval));
            triggerTime = DateTime.Now.AddSeconds(interval);

            _logger.LogInformation(
                $"Welcome train has: {welcomeQueue.Count} in queue for greeting. " +
                $"Triggering at: {triggerTime.ToString("F")} | Now: {DateTime.Now.ToString("F")}");
        }
    }
}
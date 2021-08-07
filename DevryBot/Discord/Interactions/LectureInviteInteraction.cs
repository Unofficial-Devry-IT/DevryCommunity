using System;
using System.Threading.Tasks;
using DevryBot.Options;
using DevryBot.Services;
using DisCatSharp.Entities;
using Microsoft.Extensions.Options;

namespace DevryBot.Discord.Interactions
{
    public class LectureInviteInteraction : IInteractionHandler
    {
        private readonly DiscordOptions _options;
        private readonly IWelcomeHandler _welcomeHandler;
        private readonly WelcomeOptions _welcomeOptions;
        private readonly IBot _bot;

        public LectureInviteInteraction(IOptions<DiscordOptions> options, IWelcomeHandler welcomeHandler, IOptions<WelcomeOptions> welcomeOptions, IBot bot)
        {
            _options = options.Value;
            _welcomeHandler = welcomeHandler;
            _welcomeOptions = welcomeOptions.Value;
            _bot = bot;
        }

        public async Task Handle(DiscordMember member, DiscordInteraction interaction, string[] values)
        {
            DiscordFollowupMessageBuilder responseBuilder = new DiscordFollowupMessageBuilder()
            {
                IsEphemeral = true
            };
            
            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Lecture Assistant")
                .WithDescription("Anyone who joins within the next " +
                                 $"{_welcomeOptions.InviteWelcomeDuration} hours will be shown a button to quickly join your selected roles")
                .WithImageUrl(_options.InviteImage)
                .WithFooter(_options.InviteFooter)
                .WithColor(DiscordColor.Green);
            responseBuilder.AddEmbed(embedBuilder.Build());

            await interaction.CreateFollowupMessageAsync(responseBuilder);
            await interaction.DeleteOriginalResponseAsync();
            DateTime expirationTime = DateTime.Now.AddHours(_welcomeOptions.InviteWelcomeDuration);
            foreach (var entry in values)
                _welcomeHandler.AddClass(_bot.MainGuild.Roles[ulong.Parse(entry)], expirationTime);
        }
    }
}
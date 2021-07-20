using System;
using System.Threading.Tasks;
using DevryBot.Services;
using DSharpPlusNextGen.Entities;

namespace DevryBot.Discord.Interactions
{
    public static class LectureInviteInteraction
    {
        public static async Task HandleLectureInviteSelection(DiscordMember member,
            DiscordInteraction interaction, string[] values)
        {
            DiscordFollowupMessageBuilder responseBuilder = new DiscordFollowupMessageBuilder()
            {
                IsEphemeral = true
            };
            
            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Lecture Assistant")
                .WithDescription("Anyone who joins within the next " +
                                 $"{Bot.Instance.Configuration.InviteWelcomeDuration()} hours will be shown a button to quickly join your selected roles")
                .WithImageUrl(Bot.Instance.Configuration.InviteImage())
                .WithFooter(Bot.Instance.Configuration.InviteFooter())
                .WithColor(DiscordColor.Green);
            responseBuilder.AddEmbed(embedBuilder.Build());

            await interaction.CreateFollowupMessageAsync(responseBuilder);
            await interaction.DeleteOriginalResponseAsync();
            DateTime expirationTime = DateTime.Now.AddHours(Bot.Instance.Configuration.InviteWelcomeDuration());
            foreach (var entry in values)
                WelcomeHandler.Instance.AddClass(Bot.Instance.MainGuild.Roles[ulong.Parse(entry)], expirationTime);
        }
    }
}
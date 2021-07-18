using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Services;
using DSharpPlusNextGen;
using DSharpPlusNextGen.Entities;

namespace DevryBot.Discord.Interactions
{
    public static class LectureInviteInteraction
    {
        public static async Task HandleLectureInvite(DiscordMember member, DiscordChannel channel,
            DiscordInteraction interaction)
        {
            DiscordMessageBuilder responseBuilder = new DiscordMessageBuilder();

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Sorting Hat")
                .WithFooter(member.DisplayName)
                .WithDescription(
                    "Lighten the path for your classmates! Pick the role(s) they will need when they start their journey here")
                .WithColor(DiscordColor.HotPink);

            List<DiscordSelectComponentOption> options = new();

            var roles = member.Roles
                .RemoveBlacklistedRoles(Bot.Instance.Configuration.BlacklistedRoles(channel.Guild))
                .OrderBy(x => x.Name)
                .Take(24)
                .ToList();

            foreach (var role in roles)
                options.Add(new DiscordSelectComponentOption(role.Name, role.Id.ToString()));

            string roleSelectId = $"{member.Id}_linvite";
            DiscordSelectComponent roleSelect = new DiscordSelectComponent(roleSelectId, "Roles for lecture",
                options, 1, options.Count, false);

            responseBuilder.AddComponents(roleSelect);
            responseBuilder.AddEmbed(embedBuilder.Build());

            // Reply to the button interaction that we're cool
            await interaction.CreateResponseAsync(InteractionResponseType.DefferedMessageUpdate, new());

            // This is the users followup resposne to the invitation link
            await channel.SendMessageAsync(responseBuilder);
        }

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
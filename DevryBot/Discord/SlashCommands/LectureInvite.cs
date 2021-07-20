using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Services;
using DSharpPlusNextGen.Entities;
using DSharpPlusNextGen.SlashCommands;
using Microsoft.Extensions.Logging;

namespace DevryBot.Discord.SlashCommands
{
    public class LectureInvite : SlashCommandModule
    {
        [SlashCommand("lecture-invite", "Help your fellow classmates!")]
        public static async Task Command(InteractionContext context)
        {
            try
            {
                if (!await context.ValidateGuild())
                    return;

                await context.ImThinking();
                
                DiscordWebhookBuilder responseBuilder = new();
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                    .WithTitle("Sorting Hat")
                    .WithDescription(Bot.Instance.Configuration.InviteMessage())
                    .WithFooter(Bot.Instance.Configuration.InviteFooter())
                    .WithImageUrl(Bot.Instance.Configuration.InviteImage());

                responseBuilder.AddEmbed(embedBuilder.Build());

                string menuId = $"{context.Member.Id}_linvite";

                // Allow user to select from one of their roles -- removing anything that's blacklisted
                var roles = context.Member.Roles.RemoveBlacklistedRoles(
                        Bot.Instance.Configuration.BlacklistedRoles(context.Guild))
                    .OrderBy(x => x.Name)
                    .Take(24)
                    .ToList();

                if (roles.Count == 0)
                {
                    embedBuilder.Color = DiscordColor.Yellow;
                    embedBuilder.ImageUrl = Bot.Instance.Configuration.WarningImage();
                    embedBuilder.Description =
                        "Sorry, this command assumes you have roles to choose from! Please use `/join` to join the class(es) you're in, along with your major.";

                    responseBuilder = new();
                    responseBuilder.AddEmbed(embedBuilder.Build());
                    await context.EditResponseAsync(responseBuilder);
                    return;
                }
                
                List<DiscordSelectComponentOption> options = new();

                foreach (var role in roles)
                    options.Add(new DiscordSelectComponentOption(role.Name, role.Id.ToString()));

                DiscordSelectComponent menu =
                    new DiscordSelectComponent(menuId, "Invite is for", options, 1, options.Count, false);

                responseBuilder.AddComponents(menu);
                var message = await context.EditResponseAsync(responseBuilder);

                embedBuilder = new DiscordEmbedBuilder()
                    .WithTitle("Lecture Assistant")
                    .WithDescription("Anyone who joins within the next " +
                                     $"{Bot.Instance.Configuration.InviteWelcomeDuration()} hours will be shown a button to quickly join your selected roles")
                    .WithImageUrl(Bot.Instance.Configuration.InviteImage())
                    .WithFooter(Bot.Instance.Configuration.InviteFooter())
                    .WithColor(DiscordColor.Green);

                var interaction = await Bot.Interactivity.WaitForSelectAsync(message, menuId);
                responseBuilder = new();
                responseBuilder.AddEmbed(embedBuilder.Build());
                await context.EditResponseAsync(responseBuilder);

                DateTime expirationTime = DateTime.Now.AddHours(Bot.Instance.Configuration.InviteWelcomeDuration());
                foreach (var entry in interaction.Result.Values)
                    WelcomeHandler.Instance.AddClass(context.Guild.Roles[ulong.Parse(entry)], expirationTime);
                    
                #if DEBUG
                Random random = new Random();
                int amount = random.Next(1, 10);
                for(int i = 0; i < amount; i++)
                    WelcomeHandler.Instance.AddMember(context.Member);
                #endif
            }
            catch (Exception ex)
            {
                Bot.Instance.Logger.LogError(ex, $"Error during lecture invite: {context.Member.DisplayName}");
                DiscordFollowupMessageBuilder messageBuilder = new DiscordFollowupMessageBuilder()
                {
                    IsEphemeral = true
                };

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                    .WithTitle("Error")
                    .WithDescription(ex.Message)
                    .WithImageUrl(Bot.Instance.Configuration.ErrorImage())
                    .WithColor(DiscordColor.Red);

                messageBuilder.AddEmbed(builder.Build());

                await context.FollowUpAsync(messageBuilder);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Options;
using DevryBot.Services;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using DisCatSharp.Interactivity;
using DisCatSharp.SlashCommands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevryBot.Discord.SlashCommands
{
    public class LectureInvite : SlashCommandModule
    {
        public IRoleService RoleService { get; set; }
        public ILogger<LectureInvite> Logger { get; set; }
        public IOptions<DiscordOptions> DiscordOptions { get; set; }
        public IOptions<WelcomeOptions> WelcomeOptions { get; set; }
        public IBot Bot { get; set; }
        public IWelcomeHandler WelcomeHandler { get; set; }
        

        [SlashCommand("lecture-invite", "Help your fellow classmates!")]
        public async Task Command(InteractionContext context)
        {
            try
            {
                if (!await context.ValidateGuild())
                    return;

                await context.ImThinking();
                var config = DiscordOptions.Value;
                
                DiscordWebhookBuilder responseBuilder = new();
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                    .WithTitle("Sorting Hat")
                    .WithDescription(config.InviteMessage)
                    .WithFooter(config.InviteFooter)
                    .WithImageUrl(config.InviteImage);

                responseBuilder.AddEmbed(embedBuilder.Build());

                string menuId = $"{context.Member.Id}_linvite";

                // Allow user to select from one of their roles -- removing anything that's blacklisted
                var roles = context.Member.Roles.RemoveBlacklistedRoles(RoleService.GetBlacklistedRolesDict(Bot.MainGuild.Id).Keys)
                    .OrderBy(x => x.Name)
                    .Take(24)
                    .ToList();

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
                                     $"{WelcomeOptions.Value.InviteWelcomeDuration} hours will be shown a button to quickly join your selected roles")
                    .WithImageUrl(config.InviteImage)
                    .WithFooter(config.InviteFooter)
                    .WithColor(DiscordColor.Green);

                InteractivityResult<ComponentInteractionCreateEventArgs> interaction = await Bot.Interactivity.WaitForSelectAsync(message, menuId, timeoutOverride: null);
                responseBuilder = new();
                responseBuilder.AddEmbed(embedBuilder.Build());
                await context.EditResponseAsync(responseBuilder);

                DateTime expirationTime = DateTime.Now.AddHours(WelcomeOptions.Value.InviteWelcomeDuration);
                foreach (var entry in interaction.Result.Values)
                    WelcomeHandler.AddClass(context.Guild.Roles[ulong.Parse(entry)], expirationTime);
                    
                #if DEBUG
                Random random = new Random();
                int amount = random.Next(1, 10);
                for(int i = 0; i < amount; i++)
                    WelcomeHandler.AddMember(context.Member);
                #endif
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error during lecture invite: {context.Member.DisplayName}. Roles:\n{string.Join("\n",context.Member.Roles.Select(x=>x.Name))}");
               
                DiscordFollowupMessageBuilder messageBuilder = new DiscordFollowupMessageBuilder()
                {
                    IsEphemeral = true
                };

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                    .WithTitle("Oops")
                    .WithDescription("Sorry, this command assumes you have roles to choose from! Please use `/join` to join the class(es) you're in, along with your major.")
                    .WithImageUrl(DiscordOptions.Value.WarningImage)
                    .WithColor(DiscordColor.Yellow);

                messageBuilder.AddEmbed(builder.Build());

                await context.FollowUpAsync(messageBuilder);
            }
        }
    }
}
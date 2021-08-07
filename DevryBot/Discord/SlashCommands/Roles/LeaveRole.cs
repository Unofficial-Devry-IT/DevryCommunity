using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Options;
using DevryBot.Services;
using DisCatSharp.Entities;
using DisCatSharp.SlashCommands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevryBot.Discord.SlashCommands.Roles
{
    public class LeaveRole : SlashCommandModule
    {
        public IOptions<DiscordOptions> DiscordOptions { get; set; }
        public IBot Bot { get; set; }
        public IRoleService RoleService { get; set; }
        public ILogger<LeaveRole> Logger { get; set; }

        [SlashCommand("leave", "Leave a class or two")]
        public async Task Command(InteractionContext context,
            [Option("Class", "Name of the class you want to leave")]
            string? name)
        {
            if (!await context.ValidateGuild())
                return;

            await context.ImThinking();
            
            // create the message builder
            DiscordWebhookBuilder messageBuilder = new();
            
            /*
                We need to determine if the user specified an exact match for a role
                they currently have
                
                -- Note: Discord has a restriction of 25 maximum values             
             */

            bool listAll = string.IsNullOrEmpty(name) ||
                           name.Equals("-") ||
                           name.Equals("_") ||
                           name.Equals("any") ||
                           name.Equals("list"); 
            
            var roles =
                listAll
                    ? context.Member
                        .Roles
                        .RemoveBlacklistedRoles(RoleService.GetBlacklistedRolesDict(Bot.MainGuild.Id).Keys)
                        .OrderBy(x => x.Name)
                        .Take(24)
                        .ToDictionary(x => x.Id, x => x)
                    : context.Member
                        .Roles
                        .FindRolesWithName(name)
                        .RemoveBlacklistedRoles(RoleService.GetBlacklistedRolesDict(Bot.MainGuild.Id).Keys)
                        .OrderBy(x => x.Name)
                        .Take(24)
                        .ToDictionary(x => x.Id, x => x);
            
            /*
                If we only have ONE record AND the name matches what was found -- automatically remove user from that role
             */
            if (roles.Count == 1 && roles.First().Value.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    await context.Member.RevokeRoleAsync(roles.First().Value);
                    await context.EditResponseAsync(new()
                    {
                        Content = $"{roles.First().Value.Name} has been removed"
                    });

                    return;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }    
            }

            if (roles.Count == 0)
            {
                try
                {
                    await context.EditResponseAsync(new()
                    {
                        Content = listAll ? "Nothing to remove" 
                                          : $"We could not locate anything '{name}'\n" +
                                            "Please verify the course name and try again. If the problem persists, contact a moderator"
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }
            }

            List<DiscordSelectComponentOption> options = new List<DiscordSelectComponentOption>();
            
            foreach(var role in roles)
                options.Add(new DiscordSelectComponentOption(
                    role.Value.Name,
                    role.Value.Id.ToString(),
                    "", false, null));

            string menuName = $"{context.User.Id}_{InteractionConstants.LEAVE_ROLE}";

            DiscordSelectComponent menu = new(menuName, "Select the classes you want to remove",
                options.ToArray(), 1, options.Count);

            messageBuilder.AddComponents(menu);

            try
            {
                // change our previous 'thinking' response to our actual result
                var message = await context.EditResponseAsync(messageBuilder);

                var componentInteraction =
                    await Bot.Interactivity.WaitForSelectAsync(message, menuName, timeoutOverride: null);

                Logger.LogInformation(
                    $"The user is trying to remove the following: {string.Join(", ", componentInteraction.Result.Values)}");

                List<DiscordRole> removeRoles = new();

                foreach (string roleId in componentInteraction.Result.Values)
                {
                    if (ulong.TryParse(roleId, out ulong id))
                    {
                        removeRoles.Add(roles[id]);
                        Logger.LogInformation($"Queing for removal {context.User.Username} - {roles[id].Name}");
                    }
                    else
                    {
                        Logger.LogError($"Was unable to parse {roleId} for user {context.User.Username}");
                    }
                }

                string roleText = string.Join("\n\t",
                    removeRoles.OrderBy(x => x.Name)
                                     .Select(x=>x.Name));

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                    .WithTitle("In Queue to Remove")
                    .WithDescription(roleText)
                    .WithImageUrl(DiscordOptions.Value.QueueImage);
                
                messageBuilder = new();
                messageBuilder.AddEmbed(builder.Build());
                
                await context.EditResponseAsync(messageBuilder);
                
                foreach (var role in removeRoles)
                {
                    await context.Member.RevokeRoleAsync(role);
                    await Task.Delay(500);
                }

                messageBuilder = new();
                builder.Title = "Following roles have been removed";
                builder.ImageUrl = DiscordOptions.Value.CompletedImage;
                messageBuilder.AddEmbed(builder.Build());
                
                await context.EditResponseAsync(messageBuilder);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DSharpPlusNextGen;
using DSharpPlusNextGen.Entities;
using DSharpPlusNextGen.SlashCommands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevryBot.Discord.SlashCommands
{
    public class JoinRole : SlashCommandModule
    {
        [SlashCommand("join", "Find the classes you want to join")]
        public static async Task Command(InteractionContext context,
            [Option("Class", "Name of the class or category of class you want to join")] string name)
        {
            if (!await context.ValidateGuild())
                return;            

            await context.ImThinking();
           
            // create the message builder
            DiscordWebhookBuilder messageBuilder = new();

            /*
                We need to determine if the user specified an exact match for a class             
                -- NOTE: Discord has a restriction of 25 maximum values
             */
            
            var roles = Bot.Instance.MainGuild
                .Roles
                .FindRolesWithName(name)
                .RemoveBlacklistedRoles(Bot.Instance.Configuration.BlacklistedRoles(Bot.Instance.MainGuild))
                .OrderBy(x=>x.Name)
                .Take(24)
                .ToDictionary(x=>x.Id, x=>x);

            /*
                If we only have ONE record AND the name matches what was found -- automatically add user to that role
             */ 
            if(roles.Count == 1 && roles.First().Value.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    await context.Member.GrantRoleAsync(roles.First().Value);
                    await context.EditResponseAsync(
                        new()
                        {
                            Content = $"You have been added to: {roles.First().Value.Name}"
                        });
                }
                catch (Exception ex)
                {
                    Bot.Instance.Logger.LogError(ex.Message);
                }
                
                return;
            }
            
            if (roles.Count == 0)
            {
                try
                {
                    await context.EditResponseAsync(
                        new()
                        {
                            Content = $"We could not located anything matching `{name}`.\n" +
                                      "Please verify the course name and try again. If the problem persists, contact a moderator"
                        });
                }
                catch (Exception ex)
                {
                    Bot.Instance.Logger.LogError(ex.Message);
                }
                
                return;
            }

            List<DiscordSelectComponentOption> options = new List<DiscordSelectComponentOption>();
            
            foreach(var role in roles)
                options.Add(new DiscordSelectComponentOption(
                    role.Value.Name,
                    role.Value.Id.ToString(),
                    "",
                    false,
                    null));


            string userMenuName = $"{context.User.Id}_selectRole";
            
            DiscordSelectComponent menu = new DiscordSelectComponent(userMenuName, "Select the classes you wish to join",
                options.ToArray(), 1, options.Count);

            messageBuilder.AddComponents(menu);

            try
            {
                // change our previous "thinking" response to our actual result
                var message = await context.EditResponseAsync(messageBuilder);
                
                var componentInteraction = 
                    await Bot.Interactivity.WaitForSelectAsync(message, userMenuName, null);
                
                Bot.Instance.Logger
                    .LogInformation($"The user interacted with the following: {string.Join(", ", componentInteraction.Result.Values.ToArray())}");
                
                List<DiscordRole> addRoles = new List<DiscordRole>();
                
                foreach (string roleId in componentInteraction.Result.Values)
                {
                    if (ulong.TryParse(roleId, out ulong id))
                    {
                        addRoles.Add(roles[id]);
                        Bot.Instance.Logger.LogInformation($"Queing {context.User.Username} - {roles[id].Name}");
                    }
                    else
                    {
                        Bot.Instance.Logger.LogError($"Was unable to parse {roleId} for user {context.User.Username}");
                    }
                }

                string roleText = string.Join("\n\t",
                    addRoles.OrderBy(x => x.Name)
                        .Select(x=>x.Name));

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                    .WithTitle("In Queue to Add")
                    .WithDescription(roleText)
                    .WithImageUrl(Bot.Instance.Configuration.GetValue<string>("Discord:QueueImage"));

                messageBuilder = new();
                messageBuilder.AddEmbed(builder.Build());
                
                await context.EditResponseAsync(messageBuilder);
                
                foreach (var role in addRoles)
                {
                    await context.Member.GrantRoleAsync(role);
                    await Task.Delay(500);
                }
                
                messageBuilder = new();
                builder.Title = "Following roles have been added";
                builder.ImageUrl = Bot.Instance.Configuration.GetValue<string>("Discord:CompletedImage");
                messageBuilder.AddEmbed(builder.Build());
                
                await context.EditResponseAsync(messageBuilder);
            }
            catch (Exception ex)
            {
                Bot.Instance.Logger.LogError(ex.Message);
            }
        }
    }
}
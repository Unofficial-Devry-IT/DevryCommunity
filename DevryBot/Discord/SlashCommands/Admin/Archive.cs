using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Discord.SlashCommands.Filters;
using DevryBot.Options;
using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.SlashCommands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevryBot.Discord.SlashCommands.Admin
{
    /// <summary>
    /// Goes through and retrieves all channels that have not been active for a certain amount of time
    /// -- determined by config value  "Discord:ArchiveOverDays"
    /// </summary>
    public class Archive : SlashCommandModule
    {
        public enum ArchiveReason
        {
            /// <summary>
            /// No activity has ever occurred
            /// </summary>
            NO_ACTIVITY = 0,
            
            /// <summary>
            /// No activity has occurred since a certain reference time
            /// </summary>
            NO_ACTIVITY_OVER_TIME = 1,
            
            /// <summary>
            /// Considered to be deleted due to being associated with a category to delete
            /// </summary>
            ALONG_FOR_THE_RIDE = 2
        }
        
        private struct ArchiveItem
        {
            public DiscordChannel Channel { get; set; }
            public ArchiveReason Reason { get; set; }
        }

        public IBot Bot { get; set; }
        public IOptions<ArchiveOptions> ArchiveSettings { get; set; }
        public ILogger<Archive> Logger { get; set; }


        [SlashCommand("archive", "Archive classes that have been inactive")]
        [RequireModerator]
        public async Task Command(InteractionContext context)
        {
            if (!await context.ValidateGuild())
                return;
            
            // We have to think for a bit because we're going to 
            // be querying a lot of things
            await context.ImThinking();

            // we want to avoid certain classes noted in appsettings.json
            var ignoreArchive = ArchiveSettings.Value.IgnoreArchive
                .Select(x=>x.ToLower())
                .ToList();
            
            // Find all categories -- ignore anything marked under Discord:IgnoreArchive
            var categories = Bot.MainGuild.Channels
                                                .Where(x => x.Value.Type == ChannelType.Category && !ignoreArchive.Contains(x.Value.Name.ToLower()))
                                                .Select(x => x.Value)
                                                .OrderBy(x=>x.Name)
                                                .ToList();
            
            List<ArchiveItem> archive = new();
            List<DiscordRole> rolesToRemove = new();
            DateTimeOffset referenceTime = DateTimeOffset.Now.AddDays(ArchiveSettings.Value.ArchiveOverDays);
            Dictionary<DiscordChannel, ArchiveReason> reasons = new();

            foreach (var category in categories)
            {
                reasons.Clear();
                
                int inactiveChannels = 0;
                int totalTextChannels = category.Children.Count(x => x.Type == ChannelType.Text);

                var categoryChildren = category.Children;
                
                foreach (var channel in categoryChildren)
                {
                    // We only care for text-based channels.
                    if (channel.Type != ChannelType.Text)
                        continue;
                    
                    await Task.Delay(500);

                    var recentMessages = await channel.GetMessagesAsync(1);

                    // If we can't get a message -- it's inactive -- increment and move on
                    if (recentMessages.Count == 0)
                    {
                        inactiveChannels++;
                        reasons.Add(channel, ArchiveReason.NO_ACTIVITY);
                        Logger.LogInformation($"{channel.Name} -- no activity");
                        continue;
                    }

                    // yes, this channel is inactive -- increment                    
                    if (recentMessages.First().CreationTimestamp <= referenceTime)
                    {
                        Logger.LogInformation($"{channel.Name} -- messages are older than 3 months");
                        inactiveChannels++;
                        reasons.Add(channel, ArchiveReason.NO_ACTIVITY_OVER_TIME);
                    }
                }
                
                // if the inactive channels >= totalTextChannels then we should delete the class
                if (inactiveChannels >= totalTextChannels)
                {
                    string[] splitCategoryName = category.Name
                        .Replace("-", " ")
                        .Split(" ");

                    // Must have at least the course identifier and number
                    if (splitCategoryName.Length >= 2)
                    {
                        string roleName = splitCategoryName[0] + " " + splitCategoryName[1];
                        var role = category.Guild.Roles.FirstOrDefault(x =>
                                                                x.Value.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));

                        if (role.Key > 0 && role.Value != null)
                            if(!rolesToRemove.Any(x=>x.Name.StartsWith(role.Value.Name)))
                                rolesToRemove.Add(role.Value);
                    }
                    
                    foreach (var item in categoryChildren)
                    {
                        if(reasons.ContainsKey(item))
                            archive.Add(new()
                            {
                                Channel = item,
                                Reason = reasons[item] 
                            });
                        else
                            archive.Add(new()
                            {
                                Channel = item,
                                Reason = ArchiveReason.ALONG_FOR_THE_RIDE
                            });
                    }
                    
                    archive.Add(new ArchiveItem()
                    {
                        Channel = category,
                        Reason = ArchiveReason.ALONG_FOR_THE_RIDE
                    });
                }
            }

            Logger.LogInformation($"Total of {archive.Count} channels should be removed");

            var noActivity = archive.Where(x => x.Reason == ArchiveReason.NO_ACTIVITY);
            var noActivityOverTime = archive.Where(x => x.Reason == ArchiveReason.NO_ACTIVITY_OVER_TIME);
            var alongForRide = archive.Where(x => x.Reason == ArchiveReason.ALONG_FOR_THE_RIDE);
            var parents = archive.Where(x => x.Channel.IsCategory);
            
            // Log the items that should be archived
            Logger.LogInformation($"Categories to remove: {parents.Count()}\n" +
                                               $"\t{string.Join("\n\t", parents.OrderBy(x=>x.Channel.Name).Select(x=>x.Channel.Name))}");
            Logger.LogInformation($"No Activity: {noActivity.Count()}\n" +
                                               $"\t{string.Join("\n\t", noActivity.OrderBy(x=>x.Channel.Name).Select(x=>x.Channel.Name))}");
            Logger.LogInformation($"No Activity Over Time: {noActivityOverTime.Count()}\n" +
                                               $"\t{string.Join("\n\t", noActivityOverTime.OrderBy(x=>x.Channel.Name).Select(x=>x.Channel.Name))}");
            Logger.LogInformation($"Along for the Ride: {alongForRide.Count()}\n" +
                                               $"\t{string.Join("\n\t", alongForRide.OrderBy(x=>x.Channel.Name).Select(x=>x.Channel.Name))}");
            Logger.LogInformation($"Roles to remove: {rolesToRemove.Count}\n" +
                                               $"\t{string.Join("\n\t", rolesToRemove.OrderBy(x=>x.Name).Select(x=>x.Name))}");
            // craft our response
            DiscordWebhookBuilder messageBuilder = new();
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Archive")
                .WithDescription($"No Activity: {noActivity.Count()}\n" +
                                 $"No Activity Over Time: {noActivityOverTime.Count()}\n" +
                                 $"Along for the ride: {alongForRide.Count()}\n" +
                                 $"Roles to remove: {rolesToRemove.Count}\n" +
                                 "Do you wish to archive? Please use the menu to confirm the appropriate action");
            
            // We also want to ask the user whether or not they want to go forth and delete
            messageBuilder.AddEmbed(embedBuilder.Build());

            string menuName = $"{context.User.Id}_archive";
            DiscordSelectComponent confirmationMenu = new DiscordSelectComponent(menuName,
                "Confirmation",
                new []
                {
                  new DiscordSelectComponentOption("Yes","yes",null,false),
                  new DiscordSelectComponentOption("No","no",null,false)
                },
                1, 1);

            messageBuilder.AddComponents(confirmationMenu);
            bool shouldArchive = false;
            
            try
            {
                var message = await context.EditResponseAsync(messageBuilder);

                var componentInteraction =
                    await Bot.Interactivity.WaitForSelectAsync(message, menuName, timeoutOverride: null);
                
                Logger.LogInformation($"{context.User.Username} -- interacted with confirmation menu: {string.Join(",",componentInteraction.Result.Values)}");

                if (!componentInteraction.Result.Values.Any())
                {
                    messageBuilder = new();
                    embedBuilder.Description = "No changes will be made";
                    messageBuilder.AddEmbed(embedBuilder.Build());
                    await context.EditResponseAsync(messageBuilder);
                    return;
                }
                
                shouldArchive = componentInteraction.Result.Values.First().ToLower() == "yes";

                // Give response to user -- let them know the next step
                messageBuilder = new();
                embedBuilder.Description = shouldArchive
                    ? $"Starting to arhive... this may take awhile..."
                    : "No changes will be made";
                messageBuilder.AddEmbed(embedBuilder.Build());
                
                await context.EditResponseAsync(messageBuilder);

                // Exit here if we aren't supposed to archive
                if (!shouldArchive)
                    return;
                
                DateTime startingTime = DateTime.Now;
                
                // time to start deleting
                foreach (var item in archive)
                {
                    Logger.LogWarning($"Deleting {item.Channel.Name} - {item.Reason}");
                    await item.Channel.DeleteAsync();
                    await Task.Delay(1000);
                }

                foreach (var role in rolesToRemove)
                {
                    Logger.LogWarning($"Deleting {role.Name}");
                    await role.DeleteAsync();
                    await Task.Delay(1000);
                }
                
                DateTime endTime = DateTime.Now;

                var length = endTime - startingTime;
                
                messageBuilder = new();
                embedBuilder.Description = $"Completed archival process";
                embedBuilder = embedBuilder.WithFooter($"Took: {length.ToString("c")}");
                messageBuilder.AddEmbed(embedBuilder.Build());

                await context.EditResponseAsync(messageBuilder);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }   
    }
}
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UnofficialDevryIT.Architecture.Extensions;

namespace DevryBot.Discord.SlashCommands.Admin
{
    public class CreateClass : SlashCommandModule
    {
        /// <summary>
        /// Permissions for allowed users/roles
        /// </summary>
        static Permissions ALLOWED =
            Permissions.AccessChannels |
            Permissions.AddReactions |
            Permissions.EmbedLinks |
            Permissions.ReadMessageHistory |
            Permissions.SendMessages |
            Permissions.SendTtsMessages |
            Permissions.Speak |
            Permissions.UseVoice |
            Permissions.UseVoiceDetection |
            Permissions.AttachFiles | 
            Permissions.Stream |
            Permissions.UseSlashCommands;

        /// <summary>
        /// Permissions for denied users
        /// </summary>
        static Permissions DENIED = Permissions.AccessChannels |
                             Permissions.AddReactions |
                             Permissions.Administrator |
                             Permissions.AttachFiles |
                             Permissions.BanMembers |
                             Permissions.ChangeNickname |
                             Permissions.CreateInstantInvite |
                             Permissions.DeafenMembers |
                             Permissions.EmbedLinks |
                             Permissions.KickMembers |
                             Permissions.ManageChannels |
                             Permissions.ManageEmojisAndStickers |
                             Permissions.ManageGuild |
                             Permissions.ManageMessages |
                             Permissions.ManageRoles |
                             Permissions.ManageNicknames |
                             Permissions.ManageWebhooks |
                             Permissions.MentionEveryone |
                             Permissions.MoveMembers |
                             Permissions.MuteMembers |
                             Permissions.ReadMessageHistory |
                             Permissions.SendMessages |
                             Permissions.SendTtsMessages |
                             Permissions.Speak |
                             Permissions.UseExternalEmojis |
                             Permissions.UseVoiceDetection;

        public IBot Bot { get; set; }
        public ILogger<CreateClass> Logger { get; set; }
        public IOptions<ClassCreationOptions> ClassCreationOptions { get; set; }

        [SlashCommand("create-class", "Add a new class section")]
        [RequireModerator]
        public async Task Command(InteractionContext context,
            [Option("course-identifier", "Such as CEIS, CIS, NETW")]
            string courseIdentifier,
            [Option("course-number", "The unique number for the course such as 101, 170C, etc")]
            string courseNumber,
            [Option("title", "Name of class")] string title)
        {
            if (!await context.ValidateGuild())
                return;

            await context.ImThinking();
            
            Dictionary<string, string> errors = new();
            
            /*
                We need the user to provide valid data.
                i.e -- course cannot already exist
                    Course Identifier must be at least 3 characters long
             */

            DiscordWebhookBuilder responseBuilder = new();
            
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Administration Hat")
                .WithFooter("Class Creation");
            
            // Format the text together
            string fullTitle = $"{courseIdentifier} {courseNumber} {title}";
            string roleName = $"{courseIdentifier} {courseNumber}";

            if (courseIdentifier.Length < 3)
                errors.Add("Course Identifier", "Must be at least 3 characters long");

            // Check for existing course name
            var existing = Bot.MainGuild.Channels.Any(x =>
                x.Value.Name.Equals(fullTitle, StringComparison.InvariantCultureIgnoreCase));
            
            if(existing)
                errors.Add("Duplicate", $"{fullTitle} - already exists");

            // If any errors were found --- notify the user and exit process
            if (errors.Any())
            {
                embedBuilder = embedBuilder
                    .WithDescription("Not changes made. Please address the following errors");

                foreach (var pair in errors)
                    embedBuilder = embedBuilder.AddField(pair.Key, pair.Value);
                
                Logger.LogWarning($"Creating class by {context.User.Username}\n" +
                                               $"Errors: {string.Join(", ", errors.Select(x=>x.Value))}");
                
                responseBuilder.AddEmbed(embedBuilder.Build());
                await context.EditResponseAsync(responseBuilder);
                return;
            }
            
            // need to create the category
            var categoryChannel = await Bot.MainGuild.CreateChannelCategoryAsync(fullTitle);
            var categoryRole = await Bot.MainGuild.CreateRoleAsync(roleName, ALLOWED);
            
            // apply the permissions to the category channel -- this applies to all child channels
            try
            {
                await categoryChannel.AddOverwriteAsync(categoryRole, ALLOWED, Permissions.Administrator);
                await categoryChannel.AddOverwriteAsync(Bot.MainGuild.EveryoneRole, Permissions.None, DENIED);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error while applying permissions: {ex.Message}");
            }

            try
            {
                var additionalRoles = ClassCreationOptions.Value.AdditionalRoles.ToList();
                var requiredChannels = ClassCreationOptions.Value.TextChannels.ToList();

                // Add the roles from the config to our new category
                foreach (var item in additionalRoles)
                {
                    var role = Bot.MainGuild.Roles.FirstOrDefault(x =>
                        x.Value.Name.Equals(item, StringComparison.InvariantCultureIgnoreCase));

                    if (role.Key > 0 && role.Value != null)
                        await categoryChannel.AddOverwriteAsync(role.Value, ALLOWED, Permissions.Administrator);
                }

                // Based on config -- add the required channel names
                foreach (var channelName in requiredChannels)
                {
                    await Bot.MainGuild.CreateTextChannelAsync($"{roleName}-{channelName}", categoryChannel);
                    await Task.Delay(1000);
                }

                // Create the voice channels
                for (int i = 0;
                    i < ClassCreationOptions.Value.VoiceChannels;
                    i++)
                {
                    await Bot.MainGuild.CreateVoiceChannelAsync($"{roleName}-{i + 1}", categoryChannel);
                    await Task.Delay(1000);
                }

                // Consolidate a summary report for user
                string text = $"Category: {fullTitle}\n" +
                              $"Role: {roleName}\n" +
                              $"Additional Roles:\n\t" +
                              $"{string.Join("\n\t", additionalRoles)}\n" +
                              $"Channels:\n\t" +
                              $"{string.Join("\n\t", requiredChannels)}\n";

                embedBuilder.Description = text;
                responseBuilder.AddEmbed(embedBuilder.Build());
                
                await context.EditResponseAsync(responseBuilder);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                embedBuilder.Description = "Error while processing request...";
                responseBuilder.AddEmbed(embedBuilder.Build());

                await context.EditResponseAsync(responseBuilder);
            }
        }
    }
}
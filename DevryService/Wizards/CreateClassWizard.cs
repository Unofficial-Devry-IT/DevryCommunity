using DevryService.Core;
using DevryService.Core.Util;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Wizards
{
    [WizardInfo(Name = "Admin Hat")]
    public class CreateClassWizard : Wizard
    {
        DiscordMessage WizardMessage;

        string[] AdditionalRoleNames =
        {
            "See-All-Channels"
        };

        List<DiscordRole> AdditionalRoles = new List<DiscordRole>();
        DiscordRole everyone;

        public CreateClassWizard(ulong userId, DiscordChannel channel) : base(userId, channel)
        {
            foreach (string name in AdditionalRoleNames)
            {
                DiscordRole role = channel.Guild.Roles.FirstOrDefault(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
                
                if(role != null)
                    AdditionalRoles.Add(role);
            }

            everyone = channel.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Contains("everyone"));
        }

        Permissions ALLOWED =
                Permissions.AccessChannels |
                Permissions.AddReactions |
                Permissions.EmbedLinks |
                Permissions.ReadMessageHistory |
                Permissions.SendMessages |
                Permissions.SendTtsMessages |
                Permissions.Speak |
                Permissions.UseVoice |
                Permissions.UseVoiceDetection |
                Permissions.AttachFiles;

        Permissions DENIED = Permissions.AccessChannels |
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
            Permissions.ManageEmojis |
            Permissions.ManageGuild |
            Permissions.ManageMessages |
            Permissions.ManageNicknames |
            Permissions.ManageRoles |
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


        public override async Task StartWizard(CommandContext context)
        {
            WizardMessage = await WizardReply(context, $"Time to create a new class!\n\nWhat is the course category? (i.e CEIS, NETW, etc)\n", true);
            DiscordMessage reply = await GetUserReply();
            string category, courseNumber, title;
            do
            {
                category = reply.Content;

                WizardMessage = await WizardReplyEdit(WizardMessage, $"Category: {category}\n\nWhat's the course number?", false);
                reply = await GetUserReply();

                courseNumber = reply.Content;

                WizardMessage = await WizardReplyEdit(WizardMessage, $"Category: {category}\n\nCourse Number: {courseNumber}\n\nWhat is the course title?");
                reply = await GetUserReply();

                title = reply.Content;

                await WizardReply(context, $"So you want to create a new class '{category} {courseNumber} {title}'? Reply yes/no");
                reply = await GetUserReply();
            
                // continue this process until the user confirms their creation
            } while (reply.Content.ToLower().StartsWith("n"));

            var result = await CreateClass(context, category, courseNumber, title);

            await Cleanup();

            string success = result.Item1 ? "Success" : "Failed";
            string errorMessage = result.Item2.Length > 0 ? $"Error: {result.Item2}" : "";
            string response = $"Result: {success}\n{errorMessage}";

            await WizardReply(context, response, false);
        }

        /// <summary>
        /// Determine where the new course should get placed
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="courseNumber"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        async Task<int> GetSortPosition(string identifier, string courseNumber)
        {
            var channels = await Channel.Guild.GetChannelsAsync();

            int startingIndex = 0;

            // we just want to know where the first course with this identifier lies
            // our server is not necessarily in alphabetical order in the beginning so we can't rely on that.
            for (int i = 0; i < channels.Count; i++)
            {
                if(channels[i].Name.ToLower().StartsWith(identifier.ToLower()))
                {
                    startingIndex = i;
                    break;
                }
            }

            // attempt to grab all classes that match our course identifier. We shall use this to determine where to place our new course 
            var groups = channels.Where(x => x.Name.ToLower().StartsWith(identifier.ToLower())).Select(x=>x.Name.ToLower().Replace("-"," ").Split(" "));

            List<string> names = new List<string>();

            // the second item should be the course number, if this does not exist then oh well
            foreach(var array in groups)
                if(array.Length > 1)
                    names.Add(array[1]);

            names.Add(courseNumber);

            // sort this group of items
            names.Sort();

            // determine where the course was placed within this group
            return startingIndex + names.IndexOf(courseNumber);
        }

        async Task<(bool, string)> CreateClass(CommandContext context, string identifier, string courseNumber, string title)
        {
            string errorMessage = string.Empty;
            bool success = true;

            DiscordChannel categoryChannel = await Channel.Guild.CreateChannelAsync($"{identifier} {courseNumber} {title}", DSharpPlus.ChannelType.Category);
            DiscordRole categoryRole = await Channel.Guild.CreateRoleAsync($"{identifier} {courseNumber}", ALLOWED);

            int categoryPosition = await GetSortPosition(identifier, courseNumber);
            
            // if you do not modify the position -- it will end up at the bottom of the channel list
            await categoryChannel.ModifyPositionAsync(categoryPosition);


            // Must add in the permission for our category here
            await categoryChannel.AddOverwriteAsync(categoryRole, ALLOWED, Permissions.Administrator);
            await categoryChannel.AddOverwriteAsync(everyone, Permissions.None, DENIED);

            // Now apply the same permissions above to each additional role!
            foreach (DiscordRole role in this.AdditionalRoles)
                await categoryChannel.AddOverwriteAsync(role, ALLOWED, Permissions.None);

            // Each class will need a general chat, help chat, reosources channel, and course-feedback

            await CreateTextChannel(categoryChannel, $"{identifier} {courseNumber}", "general");
            await CreateTextChannel(categoryChannel, $"{identifier} {courseNumber}", "i-need-help");
            await CreateTextChannel(categoryChannel, $"{identifier} {courseNumber}", "resources");
            await CreateTextChannel(categoryChannel, $"{identifier} {courseNumber}", "course-feedback");

            for(int i = 0; i < 3; i++)
            {
                await Channel.Guild.CreateChannelAsync($"{identifier} voice {i + 1}", ChannelType.Voice, parent: categoryChannel);
            }

            return (success, errorMessage);
        }

        async Task CreateTextChannel(DiscordChannel parent, string identifier, string title)
        {
            DiscordChannel channel = await this.Channel.Guild.CreateChannelAsync($"{identifier} {title}", ChannelType.Text, parent: parent);
            
        }
    }
}

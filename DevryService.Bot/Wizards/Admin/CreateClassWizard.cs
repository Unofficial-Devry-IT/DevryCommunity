using DevryService.Bot.Exceptions;
using DevryServices.Common.Extensions;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryService.Bot.Wizards.Admin
{
    public class CreateClassWizard : WizardBase
    {
        class ExtensionConfig
        {
            public List<string> ChannelNames { get; set; } = new List<string>();
            public List<string> AdditionalRoles { get; set; } = new List<string>();
            public int NumberOfVoiceChannels { get; set; } = 3;
        }

        /// <summary>
        /// Permissions for allowed users/roles
        /// </summary>
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

        /// <summary>
        /// Permissions for denied users
        /// </summary>
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

        List<DiscordRole> additionalRoles = new List<DiscordRole>();
        List<string> channelNames = new List<string>();
        DiscordRole everyoneRole;

        string category = string.Empty,
                number = string.Empty,
                title = string.Empty;

        int voiceChannels = 3;

        public CreateClassWizard(CommandContext context) : base(context)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            if(Config != null && !Config.ExtensionData.IsNull())
            { 
                ExtensionConfig extensionConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtensionConfig>(Config.ExtensionData);
            
                foreach(string roleName in extensionConfig.AdditionalRoles)
                {
                    var role = Context.Guild.Roles.FirstOrDefault(x => x.Value.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));

                    if(role.Value != null)
                    {
                        additionalRoles.Add(role.Value);
                    }
                }

                channelNames = extensionConfig.ChannelNames;
                voiceChannels = extensionConfig.NumberOfVoiceChannels;
            }

            everyoneRole = Context.Guild.Roles.FirstOrDefault(x => x.Value.Name.ToLower().Contains("everyone")).Value;
        }

        protected override async Task ExecuteAsync()
        {
            var originalMessage = await WithReply<string>(BasicEmbed().WithDescription($"Time to create a new class!\n\nWhat is the course category? (i.e CEIS, NETW, etc)\n").Build());

            if(originalMessage.message == null)
            {
                await SimpleReply(StoppedEmbed(), false);
                throw new StopWizardException(AuthorName);
            }

            DiscordMessage message = originalMessage.message;
            string yesNo = string.Empty;
            
            do
            {
                if(yesNo.ToLower().StartsWith("n"))
                    category = await ReplyWithEdit<string>(message, BasicEmbed().WithDescription($"Time to create a new class!\n\nWhat is the course category? (i.e CEIS, NETW, etc)\n").Build());
                else
                    category = message.Content.Trim();

                number = await ReplyWithEdit<string>(message, BasicEmbed().WithDescription($"Category: {category}\n\nWhat's the course number?").Build());
                title = await ReplyWithEdit<string>(message, BasicEmbed().WithDescription($"Category: {category}\n\nCourse Number: {number}\n\nWhat's the course title?").Build());

                yesNo = await ReplyWithEdit<string>(message, BasicEmbed().WithDescription($"So you want to create a new class '{category} {number} {title}'? Reply yes/no").Build());
            } while (yesNo.ToLower().StartsWith("n"));

            await CreateClass();
        }

        async Task<int> GetSortPosition()
        {
            var channels = await Context.Guild.GetChannelsAsync();

            channels = channels.Where(x => x.Type == ChannelType.Category).ToList();

            int actualPosition = 0;

            for(int i = 0; i < channels.Count; i++)
            {
                if(channels[i].Name.ToLower().StartsWith(category.ToLower()))
                {
                    actualPosition = channels[i].Position;
                    break;
                }
            }

            // we attempt to grab all classes that match our course category
            // we shall use this to determine where to place our new class
            var groups = channels.Where(x => x.Name.ToLower().StartsWith(category.ToLower()))
                .Select(x => x.Name.ToLower().Replace("-", " ").Split(" "));

            List<string> names = new List<string>();
            foreach (var array in groups)
                if (array.Length > 1)
                    names.Add(array[1]);

            names.Add(number);

            names.Sort();

            return actualPosition + names.IndexOf(number);
        }

        async Task CreateClass()
        {
            DiscordChannel categoryChannel = await Context.Guild.CreateChannelAsync($"{category} {number} {title}", ChannelType.Category);
            DiscordRole categoryRole = await Context.Guild.CreateRoleAsync($"{category} {number}", ALLOWED);

            // If you do not modify the position - it will end up at the bottom of the channel list
            await categoryChannel.ModifyPositionAsync(await GetSortPosition());
            await categoryChannel.AddOverwriteAsync(categoryRole, ALLOWED, Permissions.Administrator);
            await categoryChannel.AddOverwriteAsync(everyoneRole, Permissions.None, DENIED);

            // Apply role permissions for each role specified in the config
            foreach (DiscordRole role in additionalRoles)
            {
                await categoryChannel.AddOverwriteAsync(role, ALLOWED, Permissions.None);
                await Task.Delay(250);
            }

            // Create each specified channel within the config
            foreach(string name in channelNames)
            {
                await Context.Guild.CreateChannelAsync($"{category} {number} {name}", ChannelType.Text, parent: categoryChannel);
                await Task.Delay(250);
            }

            // Create the voice channels for the designated class
            for(int i = 0; i < voiceChannels; i++)
            {
                await Context.Guild.CreateChannelAsync($"{category} {number}-{i + 1}", ChannelType.Voice, parent: categoryChannel);
                await Task.Delay(250);
            }
        }

        protected override string GetDefaultAuthorIcon() => "";
        protected override string GetDefaultAuthorName() => "Admin Hat";
        protected override string GetDefaultDescription() => "Expand our inner-kingdom! Allow our knowledge seeking minions to diverge onto their requested path(s)!";
        protected override string GetDefaultHeadline() => "";
        protected override TimeSpan? GetDefaultTimeoutOverride() => null;

        protected override string GetDefaultExtensionData()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new ExtensionConfig
            {
                AdditionalRoles = new List<string>() { "See-All-Channels" },
                NumberOfVoiceChannels = 3,
                ChannelNames = new List<string>()
                {
                    "general",
                    "i-need-help",
                    "course-feedback",
                    "course-resources"
                }
            });
        }
    }
}

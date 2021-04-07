using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotApp.Classes.Commands;
using Domain.Entities.Discord;
using DSharpPlus;
using DSharpPlus.Entities;
using ChannelType = Domain.Enums.ChannelType;

namespace BotApp.Helpers
{
    internal static class ChannelHelper
    {
        static Permissions NONE = Permissions.AccessChannels |
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

        static Permissions BASIC = Permissions.Speak |
                                   Permissions.Stream |
                                   Permissions.AccessChannels |
                                   Permissions.AddReactions |
                                   Permissions.AttachFiles |
                                   Permissions.EmbedLinks |
                                   Permissions.SendMessages |
                                   Permissions.UseVoice |
                                   Permissions.UseVoiceDetection;

        private static async Task SaveChannel(DiscordChannel channel, string desc = "")
        {
            Channel entity = entity = await Bot.Instance.Context.Channels.FindAsync(channel.Id);

            if (entity == null)
            {
                entity = new Channel()
                {
                    Id = channel.Id,
                    GuildId = channel.GuildId.Value,
                    Name = channel.Name,
                    Description = desc,
                    Position = channel.Position,
                    ChannelType = ChannelType.Category
                };
                await Bot.Instance.Context.Channels.AddAsync(entity);
                await Bot.Instance.Context.SaveChangesAsync(CancellationToken.None);
            }
        }
        
        /// <summary>
        /// Generate the channels necessary for the requested class
        /// </summary>
        /// <param name="request"></param>
        internal static async Task CreateClass(CreateClassCommand request)
        {
            string roleName = $"{request.CourseCategory} {request.CourseNumber}";

            string categoryName = $"{roleName} {request.CourseName}";

            var role = await Bot.Instance.MainGuild.CreateRoleAsync(roleName, BASIC, mentionable: true);

            var allChannels = await Bot.Instance.MainGuild.GetChannelsAsync();

            var otherClass = allChannels.FirstOrDefault(x => x.Name.StartsWith(roleName));
            
            // Create the category channel that acts as the parent to the class content
            var categoryChannel = await Bot.Instance.MainGuild.CreateChannelCategoryAsync(categoryName);
            
            // Overwrites to permissions on parent - applies to all children channels
            await categoryChannel.AddOverwriteAsync(Bot.Instance.MainGuild.EveryoneRole, Permissions.None, NONE); // deny everyone all the things
            await categoryChannel.AddOverwriteAsync(role, BASIC);   
            
            // Create text channels for the class
            foreach (string name in request.TextChannels)
                await Bot.Instance.MainGuild.CreateTextChannelAsync($"{roleName}-{name.Replace(" ","-")}", categoryChannel);
            
            // Create voice channels for the class
            foreach (string name in request.VoiceChannels)
                await Bot.Instance.MainGuild.CreateVoiceChannelAsync($"{roleName}-{name.Replace(" ","-")}", categoryChannel);
            
            if (otherClass != null)
            {
                int position = otherClass.Position;
                await categoryChannel.ModifyPositionAsync(position + 1);
            }

            await SaveChannel(categoryChannel, request.Description);
        }
    }
}
using DevryServices.Common.Helpers;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryService.Bot.Services
{
    public class ManagementService
    {
        DiscordGuild _guild;
        
        public DiscordGuild Guild
        {
            get
            {
                if(_guild == null)
                    _guild = Bot.Discord.Guilds.FirstOrDefault(x => x.Value.Name.ToLower().Contains("devry")).Value;

                return _guild;
            }
        }

        public ManagementService()
        {

        }

        public async Task CreateClass(string courseIdentifier, string courseNumber, string className)
        {
            string fullName = $"{courseIdentifier} {courseNumber} {className}";

            var parentCategory = await _guild.CreateChannelCategoryAsync(fullName);
            
        }

        public async Task ChangeChannelName(ulong id, string name)
        {
            DiscordChannel channel = Guild.Channels.FirstOrDefault(x => x.Key == id).Value;

            if (channel.Name != name)
            {
                await channel.ModifyAsync((model) =>
                {
                    model.Name = name;
                });
            }
            else
                ConsoleHelper.Print($"Unable to locate {name}", ConsoleColor.Red);
        }

        public async Task ChangeChannelPosition(ulong id, int newPosition)
        {
            DiscordChannel channel = Guild.Channels.FirstOrDefault(x => x.Key == id).Value;

            if (channel != null)
            {
                await channel.ModifyPositionAsync(newPosition);
            }
            else
                ConsoleHelper.Print($"Unable to locate {id}", ConsoleColor.Red);
        }

        public Dictionary<ulong, string> GetRoles()
        {
            return Guild.Roles.ToDictionary(x => x.Key, x => x.Value.Name);
        }

        
    }
}

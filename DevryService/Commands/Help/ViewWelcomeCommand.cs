using DevryService.Core;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands.Help
{
    [Settings("viewWelcomeConfig")]
    public class ViewWelcomeCommand : BaseCommand
    {
        [Command("view-welcome")]
        public override async Task ExecuteAsync(CommandContext context)
        {
#if DEBUG
            string name = "test";
#else
            string name = "welcome";
#endif
            DiscordChannel channel = Bot.Discord.Guilds
                .FirstOrDefault(x => x.Value.Name.ToLower().Contains("devry")).Value.Channels
                .FirstOrDefault(x => x.Value.Name.ToLower().Contains(name.ToLower()))
                .Value;

            if (channel == null)
            {
                Worker.Instance.Logger.LogError($"Could not locate channel with partial text -- '{name}'");
                return;
            }

            await channel.SendMessageAsync(embed: Bot.GenerateWelcomeMessage(context.Member));
        }
    }
}

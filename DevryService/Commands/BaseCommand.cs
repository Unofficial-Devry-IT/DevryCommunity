using DevryService.Core;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands
{
    public abstract class BaseCommand : BaseCommandModule, IDiscordCommand
    {
        /// <summary>
        /// What shall be called when command is 'executed'
        /// </summary>
        /// <param name="context"></param>
        public abstract Task ExecuteAsync(CommandContext context);
    }
}

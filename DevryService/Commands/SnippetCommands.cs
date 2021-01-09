using DevryService.Core;
using DevryService.Core.Util;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands
{
    public class SnippetCommands : BaseCommandModule, IDiscordCommand, IMiscCommand
    {
        [Command("code")]
        [Settings("codeCommandConfig")]
        public async Task Code(CommandContext context)
        {
            SnippetWizard wizard = new SnippetWizard(context);

            try
            {
                wizard.Run(context);
            }
            finally
            {
                await wizard.CleanupAsync();
            }
        }
    }
}

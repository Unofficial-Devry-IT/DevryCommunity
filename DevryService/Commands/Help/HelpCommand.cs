using DevryService.Core;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands.Help
{
    [Settings("helpCommandConfig")]
    public class HelpCommand : BaseCommand
    {
        [Command("help")]
        public override async Task ExecuteAsync(CommandContext context)
        {
            HelpWizard wizard = new HelpWizard(context);

            try { wizard.Run(context); } finally { await wizard.CleanupAsync(); }
        }
    }
}

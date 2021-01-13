using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands.Misc
{
    public class CreateSnippetCommand : BaseCommand, IMiscCommand
    {
        [Command("create-snippet")]
        public override async Task ExecuteAsync(CommandContext context)
        {
            CreateSnippetWizard wizard = new CreateSnippetWizard(context);

            try { wizard.Run(); } catch { } finally{ await wizard.CleanupAsync(); }
        }
    }
}

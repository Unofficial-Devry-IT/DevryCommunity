using DevryService.Core;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DevryService.Commands.Misc
{
    [Settings("codeCommandConfig")]
    public class ViewSnippetCommand : BaseCommand, IMiscCommand
    {
        [Command("code")]
        public override async Task ExecuteAsync(CommandContext context)
        {
            SnippetWizard wizard = new SnippetWizard(context);

            try { wizard.Run(); } finally { await wizard.CleanupAsync(); }
        }
    }
}

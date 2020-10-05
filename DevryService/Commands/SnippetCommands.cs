using DevryService.Core;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands
{
    public class SnippetCommands : IDiscordCommand, IMiscCommand
    {
        [Command("code")]
        public async Task Code(CommandContext context)
        {
            SnippetWizard wizard = new SnippetWizard(context.Member.Id, context.Channel);

            try
            {
                await wizard.StartWizard(context);
            }
            catch(StopWizardException ex)
            {
                await wizard.Cleanup();
            }
            catch(Exception ex)
            {
                await wizard.Cleanup();
            }
        }
    }
}

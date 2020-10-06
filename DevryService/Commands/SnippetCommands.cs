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
    public class SnippetCommands : IDiscordCommand, IMiscCommand
    {
        [Command("code")]
        [WizardCommandInfo(Description = "Need some examples of code? No problem!",
            Emoji = ":desktop:",
            IgnoreHelpWizard = false,
            Name = "Programming Hat",
            WizardType = typeof(SnippetWizard))]
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

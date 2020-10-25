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
    public class CreateClassCommand : IDiscordCommand
    {
        [Command("create-class")]
        [WizardCommandInfo(Description = "Join your fellow minions! Select from our vast array of inner circles!",
            Name = "Admin Hat",
            WizardType = typeof(CreateClassWizard))]
        public async Task Create(CommandContext context)
        {
            CreateClassWizard wizard = new CreateClassWizard(context.Member.Id, context.Channel);

            try
            {
                await wizard.StartWizard(context);
            }
            catch (StopWizardException ex)
            {
                await wizard.Cleanup();
            }
            catch
            {
                await wizard.Cleanup();
            }
        }
    }
}

using DevryService.Core;
using DevryService.Core.Util;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands
{

    [Core.Util.Description(":classical_building:", "Join/Leave a class at your leisure!")]
    public class RoleCommands : IDiscordCommand, IAddRemoveCommand
    {
        [DSharpPlus.CommandsNext.Attributes.Command("join")]
        [WizardCommandInfo(Description="Join your fellow minions! Select from our vast array of inner circles!",
            Name ="Sorting Hat",
            WizardType = typeof(JoinRoleWizard))]
        public async Task Create(CommandContext context)
        {
            JoinRoleWizard wizard = new JoinRoleWizard(context.Member.Id, context.Channel);

            try
            {
                await wizard.StartWizard(context);
            }
            catch(StopWizardException ex)
            {
                await wizard.Cleanup();
            }
            catch
            {
                await wizard.Cleanup();
            }
        }

        [DSharpPlus.CommandsNext.Attributes.Command("leave")]
        [WizardCommandInfo(Description = "Aye, the time has come... ",
            Name = "Bouncer Hat",
            WizardType = typeof(LeaveRoleWizard))]
        public async Task Delete(CommandContext context)
        {
            LeaveRoleWizard wizard = new LeaveRoleWizard(context.Member.Id, context.Channel);

            try
            {
                await wizard.StartWizard(context);
            }
            catch(StopWizardException ex)
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

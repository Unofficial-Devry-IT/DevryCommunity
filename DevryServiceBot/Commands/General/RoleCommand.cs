using DevryServiceBot.Util;
using DevryServiceBot.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryServiceBot.Commands.General
{
    public class RoleCommand
    {
        [Command("join")]
        public async Task JoinRole(CommandContext context)
        {
            JoinRoleWizard wizard = new JoinRoleWizard(context);

            try
            {
                await wizard.StartWizard(context);
            }
            catch(StopWizardException ex)
            {
                Console.WriteLine(ex.Message);
                await wizard.Cleanup();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                await wizard.Cleanup();
            }
        }

        [Command("leave")]
        public async Task LeaveRole(CommandContext context)
        {
            LeaveRoleWizard wizard = new LeaveRoleWizard(context);

            try
            {
                await wizard.StartWizard(context);
            }
            catch(StopWizardException ex)
            {
                Console.WriteLine(ex.Message);
                await wizard.Cleanup();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                await wizard.Cleanup();
            }
        }
    }
}

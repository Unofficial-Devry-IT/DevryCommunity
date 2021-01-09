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

    [Core.Util.Description(":classical_building:", "Join/Leave a class at your leisure!")]
    public class RoleCommands : BaseCommandModule, IDiscordCommand, IAddRemoveCommand
    {
        [Command("join")]
        [Settings("joinCommandConfig")]
        public async Task Create(CommandContext context)
        {
            JoinRoleWizard wizard = new JoinRoleWizard(context);

            try
            {
                wizard.Run(context);
            }
            finally
            {
                await wizard.CleanupAsync();
            }
        }

        [Command("leave")]
        [Settings("leaveCommandConfig")]
        public async Task Delete(CommandContext context)
        {
            LeaveRoleWizard wizard = new LeaveRoleWizard(context);

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

using DevryService.Core;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands.Roles
{
    [Settings("leaveCommandConfig")]
    public class LeaveRoleCommand : BaseCommand
    {
        [Command("leave")]
        public override async Task ExecuteAsync(CommandContext context)
        {
            LeaveRoleWizard wizard = new LeaveRoleWizard(context);

            try { wizard.Run(context); } finally { await wizard.CleanupAsync(); }
        }
    }
}

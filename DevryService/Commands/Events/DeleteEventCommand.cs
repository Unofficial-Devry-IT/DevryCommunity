using DevryService.Core;
using DevryService.Wizards.Admin;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands.Events
{
    [Settings("DeleteEventCommandConfig")]
    public class DeleteEventCommand : BaseCommand
    {
        [Command("delete-event")]
        public override async Task ExecuteAsync(CommandContext context)
        {
            CommandConfig config = Core.Util.ConfigHandler.FindConfig<CommandConfig>("delete-event");
            
            if (!context.Member.Roles.Any(x => config.RestrictedRoles.Contains(x.Name)))
                return;

            DeleteEventWizard wizard = new DeleteEventWizard(context);

            try { wizard.Run(context); } finally { await wizard.CleanupAsync(); }
        }
    }
}

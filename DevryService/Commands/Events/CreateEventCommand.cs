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
    [Settings("createEventConfig")]
    public class CreateEventCommand : BaseCommand
    {
        [Command("create-event")]
        public override async Task ExecuteAsync(CommandContext context)
        {
            CreateEventWizard wizard = new CreateEventWizard(context);

            try { wizard.Run(context); } finally { await wizard.CleanupAsync(); }
        }
    }
}

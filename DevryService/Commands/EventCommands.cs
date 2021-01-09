using DevryService.Core;
using DevryService.Core.Util;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Description = DevryService.Core.Util.DescriptionAttribute;

namespace DevryService.Commands
{
    using Wizards.Admin;

    [Description(":date:", "Need an event? Want to cancel an event?")]
    public class EventCommands : BaseCommandModule, IDiscordCommand, IAddRemoveCommand
    {
        [Command("create-event")]
        [Settings("createEventConfig")]
        [RequireRoles(RoleCheckMode.Any, "Moderator","Admin")]
        public async Task Create(CommandContext context)
        {
            CreateEventWizard wizard = new CreateEventWizard(context);

            try
            {
                wizard.Run(context);
            }
            finally
            {
                await wizard.CleanupAsync();
            }

        }

        [Command("delete-event")]
        [Settings("deleteEventConfig")]
        [WizardCommandInfo( Description = "Did you make an oopsie? Need to get rid of an existing event?",
                            Emoji = "",
                            IgnoreHelpWizard = false,
                            Name = "Remove Event")]
        [RequireRoles(RoleCheckMode.Any, "Moderator", "Admin")]
        public async Task Delete(CommandContext context)
        {
            DeleteEventWizard wizard = new DeleteEventWizard(context);

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

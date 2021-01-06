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
    [Description(":date:", "Need an event? Want to cancel an event?")]
    public class EventCommands : IDiscordCommand, IAddRemoveCommand
    {
        [Command("create-event")]
        [WizardCommandInfo( Description = "Need an event? You've come to the right place!",
                            Emoji = "",
                            IgnoreHelpWizard = false,
                            Name = "Create Event")]
        [RequireRolesAttribute(RoleCheckMode.Any, "Moderator","Admin")]
        public async Task Create(CommandContext context)
        {
            CreateEventWizard wizard = new CreateEventWizard(context.Member.Id, context.Channel);

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

            await wizard.Cleanup();
        }

        [Command("delete-event")]
        [WizardCommandInfo( Description = "Did you make an oopsie? Need to get rid of an existing event?",
                            Emoji = "",
                            IgnoreHelpWizard = false,
                            Name = "Remove Event")]
        [RequireRolesAttribute(RoleCheckMode.Any, "Moderator", "Admin")]
        public async Task Delete(CommandContext context)
        {
            DeleteEventWizard wizard = new DeleteEventWizard(context.Member.Id, context.Channel);

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

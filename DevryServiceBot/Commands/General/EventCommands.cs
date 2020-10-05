using DevryServiceBot.Util;
using DevryServiceBot.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryServiceBot.Commands.General
{
    public class EventCommands
    {
        [Command("create-event")]
        public async Task CreateEvent(CommandContext context)
        {
            CreateEventWizard wizard = new CreateEventWizard(context);

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

        [Command("delete-event")]
        public async Task DeleteEvent(CommandContext context)
        {
            DeleteEventWizard wizard = new DeleteEventWizard(context);

            try
            {
                await wizard.StartWizard(context);
            }
            // The wizard automatically cleans up on success (messages are sent after cleanup)
            catch (StopWizardException ex)
            {
                Console.WriteLine(ex.Message);
                await wizard.Cleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                await wizard.Cleanup();
            }
        }
    }
}

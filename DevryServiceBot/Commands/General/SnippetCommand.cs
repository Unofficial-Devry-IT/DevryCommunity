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
    public class SnippetCommand
    {
        [Command("snippet")]
        public async Task Snippet(CommandContext context)
        {
            SnippetWizard wizard = new SnippetWizard(context);
            
            try
            {
                await wizard.StartWizard(context);
            }
            catch(StopWizardException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                await wizard.Cleanup();
            }
        }
    }
}

using DevryService.Core;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevryService;
using Microsoft.Extensions.Logging;

namespace DevryService.Commands.Roles
{
    [Settings("JoinRoleCommandConfig")]
    public class JoinRoleCommand : BaseCommand
    {
        [Command("join")]
        public override async Task ExecuteAsync(CommandContext context)
        {
            JoinRoleWizard wizard = new JoinRoleWizard(context);

            try 
            {  
               wizard.Run(); 
            } 
            catch(Exception ex)
            { 
               Worker.Instance.Logger.LogError(ex.ToString());
               System.Console.WriteLine(ex);
            } finally { await wizard.CleanupAsync(); }
        }
    }
}

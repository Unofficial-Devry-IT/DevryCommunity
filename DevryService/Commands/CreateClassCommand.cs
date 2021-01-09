using DevryService.Core;
using DevryService.Core.Util;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DevryService.Commands
{
    using Wizards.Admin;

    public class CreateClassCommand : BaseCommandModule, IDiscordCommand
    {   
        [Command("create-class")]
        [Settings("createClassConfig")]
        public async Task Create(CommandContext context)
        {            
            CreateClassWizard wizard = new CreateClassWizard(context);

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

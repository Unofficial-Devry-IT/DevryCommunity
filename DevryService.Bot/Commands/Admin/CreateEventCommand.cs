using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryService.Bot.Commands.Admin
{
    public class CreateEventCommand : BaseCommand
    {
        [Command("create-event")]
        public override async Task ExecuteAsync(CommandContext context)
        {
            
        }

        protected override string GetDescription()
        {
            throw new NotImplementedException();
        }

        protected override string GetEmoji()
        {
            throw new NotImplementedException();
        }

        protected override IList<string> GetRestrictedRoles()
        {
            throw new NotImplementedException();
        }

        protected override TimeSpan? GetTimeoutOverride()
        {
            throw new NotImplementedException();
        }
    }
}

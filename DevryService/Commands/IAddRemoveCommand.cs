using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands
{
    public interface IAddRemoveCommand
    {
        Task Create(CommandContext context);
        Task Delete(CommandContext context);
    }
}

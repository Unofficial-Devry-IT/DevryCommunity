using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Database.Contracts
{
    public interface IDiscordCommand
    {
        string CommandName { get; }
        IList<string> RestrictedRoles { get; }
        string Description { get; }
        string Emoji { get; }
        TimeSpan? TimeoutOverride { get; }
    }
}

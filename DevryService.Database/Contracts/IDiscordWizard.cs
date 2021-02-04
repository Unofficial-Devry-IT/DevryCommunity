using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Database.Contracts
{
    public interface IDiscordWizard
    {
        string AuthorName { get; }
        string AuthorIcon { get; }
        string Headline { get; }
        string Description { get; }
        TimeSpan? TimeoutOverride { get; }
        string ExtensionData { get; }
    }
}

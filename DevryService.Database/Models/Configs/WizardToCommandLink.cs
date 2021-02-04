using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Database.Models.Configs
{
    [Serializable]
    public class WizardToCommandLink
    {
        public string DiscordCommand { get; set; }
        public CommandConfig CommandConfig { get; set; }
    }
}

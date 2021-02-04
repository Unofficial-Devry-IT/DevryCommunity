using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Database.Models.Configs
{
    public class CommandConfig : CommonConfig
    {
        public string DiscordCommand { get; set; }
        public bool IgnoreHelpWizard { get; set; } = false;
        public List<string> RestrictedRoles = new List<string>();
    }
}

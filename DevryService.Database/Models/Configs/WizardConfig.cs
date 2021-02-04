using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Database.Models.Configs
{
    public class WizardConfig : CommonConfig
    {
        public string Headline { get; set; }
        public bool AcceptAnyUser { get; set; } = false;
        public bool MessageRequireMention { get; set; } = false;

        public string CommandConfigId { get; set; }
        public CommandConfig CommandConfig { get; set; }
    }
}

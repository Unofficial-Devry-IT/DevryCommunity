using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Database.Models.Configs
{
    public class EmbedConfig : MessageConfig
    {
        public string Title { get; set; }
        public string Footer { get; set; }
        public List<string> Fields { get; set; }
    }
}

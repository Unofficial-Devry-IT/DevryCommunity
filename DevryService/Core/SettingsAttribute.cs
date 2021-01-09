using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core
{
    public class CommandConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Emoji { get; set; }
        public bool IgnoreHelpWizard { get; set; } = false;
        public string Icon { get; set; }
        public List<string> RestrictedRoles = new List<string>();
    }

    public class EmbedConfig : MessageConfig
    {
        public string Title { get; set; }
        public string Footer { get; set; }
        public List<string> Fields { get; set; }
    }

    public class MessageConfig : CommandConfig
    {
        public string Contents { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SettingsAttribute : Attribute
    {
        public SettingsAttribute(string configKey)
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core
{
    public class CommandConfig : CommonConfig
    {
        public bool IgnoreHelpWizard { get; set; } = false;
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

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SettingsAttribute : Attribute
    {
        public string ConfigKey { get; set; }
        public Type BelongsToWizard { get; set; }

        public SettingsAttribute(string configKey, Type wizardType = null)
        {
            this.ConfigKey = configKey;
            this.BelongsToWizard = wizardType;
        }
    }
}

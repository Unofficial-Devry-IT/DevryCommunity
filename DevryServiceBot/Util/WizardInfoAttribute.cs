using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevryServiceBot.Util
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WizardInfoAttribute : Attribute
    {
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string Description { get; set; }
        public string ReactionEmoji { get; set; }
        public string Group { get; set; }
        public string GroupDescription { get; set; }
        public bool IgnoreHelpWizard { get; set; }
        public string CommandName { get; set; }
    }
}

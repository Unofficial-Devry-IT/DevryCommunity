using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core
{
    [Serializable]
    public abstract class CommonConfig
    {
        public TimeSpan? TimeoutOverride { get; set; } = null;
        public string AuthorName { get; set; }
        public string AuthorIcon { get; set; }
        public string Description { get; set; }
        public string ReactionEmoji { get; set; }
    }

    [Serializable]
    public class WizardToCommandLink
    {
        public string DiscordCommand { get; set; }
        public CommandConfig CommandConfig { get; set; }
    }

    [Serializable]
    public abstract class WizardConfig : CommonConfig
    {
        /// <summary>
        /// Does this wizard allow 'any' user to respond/use it?
        /// </summary>
        public bool AcceptAnyUser { get; set; } = false;

        /// <summary>
        /// Does the wizard require a mention to be used in order to 'accept' that as valid input?
        /// </summary>
        public bool MessageRequireMention { get; set; }  = false;

        /// <summary>
        /// Text that appears next to the name
        /// </summary>
        public string Headline { get; set; }

        public WizardToCommandLink UsesCommand { get; set; }

    }

    [Serializable]
    public class OptionsBaseConfig
    {
        /// <summary>
        /// Represents the page (not implemented)
        /// </summary>
        public int Page { get; set; }

        public List<string> RestrictedRoles { get; set; } = new List<string>();

        /// <summary>
        /// Determines if this option is a yes/no 
        /// </summary>
        public YesNoOptionConfig YesNoBetween { get; set; } = null;

        /// <summary>
        /// Command/Wizard that shall be ran
        /// </summary>
        public RunCommandConfig RunCommand { get; set; } = null;
    }

    [Serializable]
    public class RunCommandConfig
    {
        public string CommandName { get; set; }
        public string Emoji { get; set; }
    }

    [Serializable]
    public class YesNoOptionConfig
    {
        public string Emoji { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Yes { get; set; }
        public string YesEmoji { get; set; } = ":white_check_mark:";
        public string No { get; set; }
        public string NoEmoji { get; set; } = ":negative_squared_cross_mark:";
    }
}

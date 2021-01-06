using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core
{
    [Serializable]
    public abstract class WizardConfig
    {
        /// <summary>
        /// Will we override the default timeout of 2 minutes?
        /// </summary>
        public TimeSpan? TimeoutOverride { get; set; } = null;

        /// <summary>
        /// Does this wizard allow 'any' user to respond/use it?
        /// </summary>
        public bool AcceptAnyUser { get; set; } = false;

        /// <summary>
        /// Does the wizard require a mention to be used in order to 'accept' that as valid input?
        /// </summary>
        public bool MessageRequireMention { get; set; }  = false;

        /// <summary>
        /// Name of the wizard. I.E Sorting Hat
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// URL/Icon to use as the author
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Text that appears next to the name
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of wizard
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Emoji which represents this action in wizard menus
        /// </summary>
        public string ReactionEmoji { get; set; }
    }
}

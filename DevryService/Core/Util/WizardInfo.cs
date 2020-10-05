using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DevryService.Core.Util
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WizardInfo : Attribute
    {
        /// <summary>
        /// General Name of commands available to class
        /// </summary>
        public string Name { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// Icon which represents this particular subset of commands
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// Description that shall appear in the wizard
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Should these set of commands (within the class) be ignored?
        /// </summary>
        public bool IgnoreHelpWizard { get; set; }

        /// <summary>
        /// Emoji which represents this section of the app
        /// </summary>
        public string Emoji { get; set; }
        public Type CommandType { get; set; }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class WizardCommandInfo : Attribute
    {
        /// <summary>
        /// Should this command be ignored by the wizard?
        /// </summary>
        /// <remarks>
        /// If the parent class is not ignored, this is how you can 
        /// tag individual commands to not appear in wizard
        /// </remarks>
        public bool IgnoreHelpWizard { get; set; }
        public string Emoji { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Type WizardType { get; set; }
    }
}

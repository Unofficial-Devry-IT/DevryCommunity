using System;
using System.Collections.Generic;
using System.Text;

namespace DevryService.Core.Util
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WizardLink : Attribute
    {
        public Type WizardType { get; set; }

        public WizardLink(Type type) { WizardType = type; }
    }
}

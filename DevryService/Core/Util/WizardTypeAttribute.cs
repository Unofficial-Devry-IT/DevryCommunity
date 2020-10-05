using System;
using System.Collections.Generic;
using System.Text;

namespace DevryService.Core.Util
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WizardTypeAttribute : Attribute
    {
        public Type WizardType { get; set; }
        public WizardTypeAttribute(Type type) { this.WizardType = type; }
    }
}

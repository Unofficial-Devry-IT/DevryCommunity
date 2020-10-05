using System;
using System.Collections.Generic;
using System.Text;

namespace DevryService.Core
{
    public class StopWizardException : Exception
    {
        public string WizardName { get; private set; }
        public StopWizardException(string name) { this.WizardName = name; }
    }
}

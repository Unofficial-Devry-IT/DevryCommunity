using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryService.Bot.Exceptions
{
    public class StopWizardException : Exception
    {
        public string Name { get; private set; }

        public StopWizardException(string name)
        {
            this.Name = name;
        }

        public StopWizardException(string name, string message) : base(message)
        {
            this.Name = name;
        }
    }
}

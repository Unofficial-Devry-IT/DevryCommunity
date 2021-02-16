using System;

namespace BotApp.Common.Exceptions
{
    public class StopWizardException : Exception
    {
        public string Name { get; }

        public StopWizardException(string name)
        {
            Name = name;
        }

        public StopWizardException(string name, string message) : base(message)
        {
            Name = name;
        }
    }
}
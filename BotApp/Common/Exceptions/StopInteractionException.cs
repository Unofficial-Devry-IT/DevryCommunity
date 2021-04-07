using System;

namespace BotApp.Common.Exceptions
{
    public class StopInteractionException : Exception
    {
        public string Name { get; }

        public StopInteractionException(string name)
        {
            Name = name;
        }

        public StopInteractionException(string name, string message) : base(message)
        {
            Name = name;
        }
    }
}
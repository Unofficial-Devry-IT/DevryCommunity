using Domain.Common;
using Domain.Entities.Configs;

namespace Domain.Events.CommandConfigs
{
    public class CommandConfigCreatedEvent : DomainEvent
    {
        public CommandConfig CommandConfig { get; }

        public CommandConfigCreatedEvent(CommandConfig config)
        {
            CommandConfig = config;
        }
    }
}
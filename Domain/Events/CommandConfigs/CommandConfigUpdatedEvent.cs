using Domain.Common;
using Domain.Entities.Configs;

namespace Domain.Events.CommandConfigs
{
    public class CommandConfigUpdatedEvent : DomainEvent
    {
        public CommandConfig CommandConfig { get; }

        public CommandConfigUpdatedEvent(CommandConfig config)
        {
            CommandConfig = config;
        }
    }
}
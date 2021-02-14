using Domain.Common;
using Domain.Entities.Configs;

namespace Domain.Events.CommandConfigs
{
    public class CommandConfigDeletedEvent : DomainEvent
    {
        public CommandConfig CommandConfig { get; }

        public CommandConfigDeletedEvent(CommandConfig config)
        {
            CommandConfig = config;
        }
    }
}
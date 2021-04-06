using Domain.Common;
using Domain.Entities.Configs;

namespace Domain.Events.CommandConfigs
{
    public class CommandConfigDeletedEvent : DomainEvent
    {
        public Config CommandConfig { get; }

        public CommandConfigDeletedEvent(Config config)
        {
            CommandConfig = config;
        }
    }
}
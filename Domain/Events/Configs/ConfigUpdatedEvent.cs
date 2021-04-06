using Domain.Common;
using Domain.Entities.Configs;

namespace Domain.Events.CommandConfigs
{
    public class ConfigUpdatedEvent : DomainEvent
    {
        public Config CommandConfig { get; }

        public ConfigUpdatedEvent(Config config)
        {
            CommandConfig = config;
        }
    }
}
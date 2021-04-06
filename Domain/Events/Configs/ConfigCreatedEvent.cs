using Domain.Common;
using Domain.Entities.Configs;

namespace Domain.Events.CommandConfigs
{
    public class ConfigCreatedEvent : DomainEvent
    {
        public Config Config { get; }

        public ConfigCreatedEvent(Config config)
        {
            Config = config;
        }
    }
}
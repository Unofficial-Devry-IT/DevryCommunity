using Domain.Common.Models;
using Domain.Entities;

namespace Domain.Events.Configs
{
    /// <summary>
    /// Event when config is updated
    /// </summary>
    public class ConfigUpdatedEvent : DomainEvent
    {
        /// <summary>
        /// Config which got updated
        /// </summary>
        public Config Config { get; }

        public ConfigUpdatedEvent(Config config)
        {
            Config = config;
        }
    }
}
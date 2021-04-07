using Domain.Common.Models;
using Domain.Entities;

namespace Domain.Events.Configs
{
    /// <summary>
    /// Event when config gets created
    /// </summary>
    public class ConfigCreatedEvent : DomainEvent
    {
        /// <summary>
        /// Config which got created
        /// </summary>
        public Config Config { get; }

        public ConfigCreatedEvent(Config config)
        {
            Config = config;
        }
    }
}
using Domain.Common.Models;
using Domain.Entities;

namespace Domain.Events.Configs
{
    /// <summary>
    /// Event when config gets deleted
    /// </summary>
    public class ConfigDeletedEvent : DomainEvent
    {
        /// <summary>
        /// Config which got deleted
        /// </summary>
        public Config Config { get; }

        public ConfigDeletedEvent(Config config)
        {
            Config = config;
        }
    }
}
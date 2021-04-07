using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common.Models;
using Domain.Enums;
using Domain.Events.Channels;

namespace Domain.Entities.Discord
{
    public class Channel : AuditableEntity, IHasDomainEvent
    {
        /// <summary>
        /// Unique ID for the discord channel
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Guild / Server ID 
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Name / text that appears in discord
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description for the channel
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether the channel is text, voice, category
        /// </summary>
        public ChannelType ChannelType { get; set; }

        /// <summary>
        /// Position of channel within discord's list
        /// </summary>
        public int Position { get; set; }

        private bool _done;

        public bool Done
        {
            get => _done;
            set
            {
                if (value && !_done)
                    DomainEvents.Add(new ChannelCreatedEvent(this));
                
                _done = value;
            }
        }

        [NotMapped] 
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
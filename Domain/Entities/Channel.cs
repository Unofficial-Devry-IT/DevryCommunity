using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Enums;
using Domain.Events.Channels;

namespace Domain.Entities
{
    public class Channel : AuditableEntity, IHasDomainEvent
    {
        /// <summary>
        /// Unique Id / PK of channel
        /// </summary>
        public ulong Id { get; set; }
        
        /// <summary>
        /// Discord server Id
        /// </summary>
        public ulong GuildId { get; set; }
        
        /// <summary>
        /// Name that appears in discord
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of what the channel represents
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether the channel is text, voice, or category
        /// </summary>
        public ChannelType ChannelType { get; set; }
        
        /// <summary>
        /// Position of channel in discord guild
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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Tasks.Scheduling;
using Domain.Common.Models;
using Domain.Entities.Discord;
using Domain.Events.Reminders;

namespace Domain.Entities
{
    /// <summary>
    /// Represents Events/Notifications that shall occur at
    /// certain intervals
    /// </summary>
    public class Reminder : EntityBase, IScheduledTask, IHasDomainEvent
    {
        /// <summary>
        /// GUILD/Server ID this reminder pertains to
        /// </summary>
        public ulong GuildId { get; set; }
        
        /// <summary>
        /// Channel ID this reminder shall appear in
        /// </summary>
        public ulong ChannelId { get; set; }
        
        /// <summary>
        /// Backreference --> foreign key is <see cref="ChannelId"/>
        /// </summary>
        public Channel Channel { get; set; }
        
        /// <summary>
        /// CronString for when this reminder shall get invoked
        /// </summary>
        public string Schedule { get; set; }
        
        /// <summary>
        /// Unique ID for this reminder
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Name/Title that shall appear
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Text that shall appear within the body/message area
        /// </summary>
        public string Contents { get; set; }
        
        /// <summary>
        /// Next scheduled time this event should run
        /// </summary>
        public DateTime NextRunTime { get; set; }

        /// <summary>
        /// Have the events been published
        /// </summary>
        private bool _done;

        public bool Done
        {
            get => _done;
            set
            {
                if (value && !_done)
                    DomainEvents.Add(new ReminderCreatedEvent(this));
                
                _done = value;
            }
        }

        [NotMapped] public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
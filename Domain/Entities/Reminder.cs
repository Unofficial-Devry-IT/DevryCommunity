using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DevryServices.Common.Models;
using DevryServices.Common.Tasks.Scheduling;
using Domain.Common;
using Domain.Events.Reminders;

namespace Domain.Entities
{
    public class Reminder : EntityBase, IScheduledTask, IHasDomainEvent
    {
        public Reminder()
        {
            Id = Guid.NewGuid().ToString();
        }
        
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public Channel Channel { get; set; }
        public string Schedule { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public DateTime NextRunTime { get; set; }

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

        [NotMapped]
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
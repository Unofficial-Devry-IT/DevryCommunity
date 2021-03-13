using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Events.Roles;

namespace Domain.Entities
{
    public class Role : IHasDomainEvent
    {
        public ulong Id { get; set; }

        public ulong GuildId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        private bool _done;

        public bool Done
        {
            get => _done;
            set
            {
                if (value && !_done)
                    DomainEvents.Add(new RoleCreatedEvent(this));
                
                _done = value;
            }
        }
        
        [NotMapped]
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
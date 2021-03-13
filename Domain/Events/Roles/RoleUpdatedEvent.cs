using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Roles
{
    public class RoleUpdatedEvent : DomainEvent
    {
        public  Role Role { get; }

        public RoleUpdatedEvent(Role role)
        {
            Role = role;
        }
    }
}
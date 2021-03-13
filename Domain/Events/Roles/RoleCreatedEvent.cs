using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Roles
{
    public class RoleCreatedEvent : DomainEvent
    {
        public RoleCreatedEvent(Role role)
        {
            Role = role;
        }

        public Role Role { get; }
    }
}
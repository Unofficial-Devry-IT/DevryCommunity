using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Roles
{
    public class RoleDeletedEvent : DomainEvent
    {
        public Role Role { get; }

        public RoleDeletedEvent(Role role)
        {
            Role = role;
        }
    }
}
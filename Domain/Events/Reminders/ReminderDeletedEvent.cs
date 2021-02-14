using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Reminders
{
    public class ReminderDeletedEvent : DomainEvent
    {
        public Reminder Reminder { get; }

        public ReminderDeletedEvent(Reminder reminder)
        {
            Reminder = reminder;
        }
    }
}
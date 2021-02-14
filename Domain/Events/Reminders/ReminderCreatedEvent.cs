using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Reminders
{
    public class ReminderCreatedEvent : DomainEvent
    {
        public Reminder Reminder { get; }

        public ReminderCreatedEvent(Reminder reminder)
        {
            Reminder = reminder;
        }
    }
}
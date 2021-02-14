using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Reminders
{
    public class ReminderUpdatedEvent : DomainEvent
    {
        public Reminder Reminder { get; }

        public ReminderUpdatedEvent(Reminder reminder)
        {
            Reminder = reminder;
        }

    }
}
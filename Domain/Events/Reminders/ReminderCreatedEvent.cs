using Domain.Common.Models;
using Domain.Entities;

namespace Domain.Events.Reminders
{
    /// <summary>
    /// Event when reminder gets created
    /// </summary>
    public class ReminderCreatedEvent : DomainEvent
    {
        /// <summary>
        /// Reminder which got created
        /// </summary>
        public Reminder Reminder { get; }

        public ReminderCreatedEvent(Reminder reminder)
        {
            Reminder = reminder;
        }
    }
}
using Domain.Common.Models;
using Domain.Entities;

namespace Domain.Events.Reminders
{
    /// <summary>
    /// Event for when a reminder gets updated
    /// </summary>
    public class ReminderUpdatedEvent : DomainEvent
    {
        /// <summary>
        /// Reminder which got updated
        /// </summary>
        public Reminder Reminder { get; }

        public ReminderUpdatedEvent(Reminder reminder)
        {
            Reminder = reminder;
        }
    }
}
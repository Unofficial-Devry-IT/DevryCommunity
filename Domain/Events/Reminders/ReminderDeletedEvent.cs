using Domain.Common.Models;
using Domain.Entities;

namespace Domain.Events.Reminders
{
    /// <summary>
    /// Event for when reminder gets deleted
    /// </summary>
    public class ReminderDeletedEvent : DomainEvent
    {
        /// <summary>
        /// Reminder which got deleted
        /// </summary>
        public Reminder Reminder { get; }

        public ReminderDeletedEvent(Reminder reminder)
        {
            Reminder = reminder;
        }
    }
}
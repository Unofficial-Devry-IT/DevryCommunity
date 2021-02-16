using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using BotApp.Services.Reminders;
using Domain.Events.Reminders;
using MediatR;

namespace BotApp.NotificationHandlers.Reminders
{
    public class DeleteEventNoficationHandler : INotificationHandler<DomainEventNotification<ReminderDeletedEvent>>
    {
        private readonly ReminderBackgroundService _reminderBackgroundService;

        public DeleteEventNoficationHandler(ReminderBackgroundService reminderService)
        {
            _reminderBackgroundService = reminderService;
        }

        public Task Handle(DomainEventNotification<ReminderDeletedEvent> notification,
            CancellationToken cancellationToken)
        {
            _reminderBackgroundService?.RemoveTask(notification.DomainEvent.Reminder.Id);

            return Task.CompletedTask;
        }
    }
}
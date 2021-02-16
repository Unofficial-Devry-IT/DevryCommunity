using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using BotApp.Services.Reminders;
using Domain.Events.Reminders;
using MediatR;

namespace BotApp.NotificationHandlers.Reminders
{
    public class UpdateEventNotificationHandler : INotificationHandler<DomainEventNotification<ReminderUpdatedEvent>>
    {
        private readonly ReminderBackgroundService _reminderBackgroundService;

        public UpdateEventNotificationHandler(ReminderBackgroundService reminderBackgroundService)
        {
            _reminderBackgroundService = reminderBackgroundService;
        }

        public Task Handle(DomainEventNotification<ReminderUpdatedEvent> notification, CancellationToken cancellationToken)
        { 
            _reminderBackgroundService?.UpdateTask(notification.DomainEvent.Reminder);

            return Task.CompletedTask;
        }
    }
}
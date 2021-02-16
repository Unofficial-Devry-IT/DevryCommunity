using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using BotApp.Services.Reminders;
using Domain.Events.Reminders;
using MediatR;

namespace BotApp.NotificationHandlers.Reminders
{
    public class CreateEventNotificationHandler : INotificationHandler<DomainEventNotification<ReminderCreatedEvent>>
    {
        private readonly ReminderBackgroundService _reminderBackgroundService;

        public CreateEventNotificationHandler(ReminderBackgroundService reminderService)
        {
            _reminderBackgroundService = reminderService;
        }

        public Task Handle(DomainEventNotification<ReminderCreatedEvent> notification,
            CancellationToken cancellationToken)
        {
            _reminderBackgroundService?.AddTask(notification.DomainEvent.Reminder);

            return Task.CompletedTask;
        }
    }
}
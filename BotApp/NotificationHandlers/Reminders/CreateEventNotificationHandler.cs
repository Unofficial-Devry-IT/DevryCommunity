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
        public Task Handle(DomainEventNotification<ReminderCreatedEvent> notification,
            CancellationToken cancellationToken)
        {
            ReminderBackgroundService.Instance?.AddTask(notification.DomainEvent.Reminder);

            return Task.CompletedTask;
        }
    }
}
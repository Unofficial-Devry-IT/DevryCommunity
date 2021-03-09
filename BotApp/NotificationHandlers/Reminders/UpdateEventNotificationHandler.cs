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
        public Task Handle(DomainEventNotification<ReminderUpdatedEvent> notification, CancellationToken cancellationToken)
        { 
            ReminderBackgroundService.Instance?.UpdateTask(notification.DomainEvent.Reminder);

            return Task.CompletedTask;
        }
    }
}
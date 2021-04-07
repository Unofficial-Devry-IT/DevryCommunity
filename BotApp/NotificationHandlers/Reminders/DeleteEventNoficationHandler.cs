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
        public Task Handle(DomainEventNotification<ReminderDeletedEvent> notification,
            CancellationToken cancellationToken)
        {
            ReminderBackgroundService.Instance?.RemoveTask(notification.DomainEvent.Reminder.Id.ToString());

            return Task.CompletedTask;
        }
    }
}
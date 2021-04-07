using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Reminders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Reminders.Notifications
{
    /// <summary>
    /// Taps into the <see cref="ReminderDeletedEvent"/> for logging purposes
    /// </summary>
    public class ReminderDeletedNotificationHandler : INotificationHandler<DomainEventNotification<ReminderDeletedEvent>>
    {
        private ILogger<ReminderDeletedNotificationHandler> _logger;

        public ReminderDeletedNotificationHandler(ILogger<ReminderDeletedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ReminderDeletedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            
            _logger.LogInformation(($"Domain Event: {domainEvent.GetType().Name}"));

            return Task.CompletedTask;
        }
    }
}
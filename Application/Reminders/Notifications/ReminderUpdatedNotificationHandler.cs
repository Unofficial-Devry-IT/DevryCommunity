using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Reminders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Reminders.Notifications
{
    /// <summary>
    /// Taps into the <see cref="ReminderUpdatedEvent"/> for logging purposes
    /// </summary>
    public class ReminderUpdatedNotificationHandler : INotificationHandler<DomainEventNotification<ReminderUpdatedEvent>>
    {
        private readonly ILogger<ReminderUpdatedNotificationHandler> _logger;

        public ReminderUpdatedNotificationHandler(ILogger<ReminderUpdatedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ReminderUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            
            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
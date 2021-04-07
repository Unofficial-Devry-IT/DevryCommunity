using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Reminders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Reminders.Notifications
{
    /// <summary>
    /// Taps into the <see cref="ReminderCreatedEvent"/> event for logging purposes
    /// </summary>
    public class ReminderCreatedNotificationHandler : INotificationHandler<DomainEventNotification<ReminderCreatedEvent>>
    {
        private readonly ILogger<ReminderCreatedNotificationHandler> _logger;

        public ReminderCreatedNotificationHandler(ILogger<ReminderCreatedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ReminderCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            
            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
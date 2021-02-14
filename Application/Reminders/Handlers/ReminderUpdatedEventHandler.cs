using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Reminders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Reminders.Handlers
{
    public class ReminderUpdatedEventHandler : INotificationHandler<DomainEventNotification<ReminderUpdatedEvent>>
    {
        private readonly ILogger<ReminderUpdatedEventHandler> _logger;

        public ReminderUpdatedEventHandler(ILogger<ReminderUpdatedEventHandler> logger)
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
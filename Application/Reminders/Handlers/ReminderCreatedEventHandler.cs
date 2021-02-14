using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Reminders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Reminders.Handlers
{
    public class ReminderCreatedEventHandler : INotificationHandler<DomainEventNotification<ReminderCreatedEvent>>
    {
        private readonly ILogger<ReminderCreatedEventHandler> _logger;

        public ReminderCreatedEventHandler(ILogger<ReminderCreatedEventHandler> logger)
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
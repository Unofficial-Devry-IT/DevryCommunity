using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Reminders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Reminders.Handlers
{
    public class ReminderDeletedEventHandler : INotificationHandler<DomainEventNotification<ReminderDeletedEvent>>
    {
        private readonly ILogger<ReminderDeletedEventHandler> _logger;

        public ReminderDeletedEventHandler(ILogger<ReminderDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ReminderDeletedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
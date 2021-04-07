using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Configs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Configs.Notifications
{
    public class ConfigCreatedNotificationHandler : INotificationHandler<DomainEventNotification<ConfigCreatedEvent>>
    {
        private readonly ILogger<ConfigCreatedNotificationHandler> _logger;

        public ConfigCreatedNotificationHandler(ILogger<ConfigCreatedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ConfigCreatedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
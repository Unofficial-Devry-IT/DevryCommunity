using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Configs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Configs.Notifications
{
    public class ConfigUpdatedNotificationHandler : INotificationHandler<DomainEventNotification<ConfigUpdatedEvent>>
    {
        private readonly ILogger<ConfigUpdatedNotificationHandler> _logger;

        public ConfigUpdatedNotificationHandler(ILogger<ConfigUpdatedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ConfigUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            
            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
} 
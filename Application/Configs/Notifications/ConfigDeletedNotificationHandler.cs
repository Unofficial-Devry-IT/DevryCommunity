using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Configs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Configs.Notifications
{
    public class ConfigDeletedNotificationHandler : INotificationHandler<DomainEventNotification<ConfigDeletedEvent>>
    {
        private readonly ILogger<ConfigDeletedNotificationHandler> _logger;

        public ConfigDeletedNotificationHandler(ILogger<ConfigDeletedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ConfigDeletedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            
            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
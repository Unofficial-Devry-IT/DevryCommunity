using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.CommandConfigs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.CommandConfigs.Handlers
{
    public class UpdateConfigHandler : INotificationHandler<DomainEventNotification<ConfigUpdatedEvent>>
    {
        private readonly ILogger<UpdateConfigHandler> _logger;

        public UpdateConfigHandler(ILogger<UpdateConfigHandler> logger)
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
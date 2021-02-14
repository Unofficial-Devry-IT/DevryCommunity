using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.CommandConfigs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.CommandConfigs.Handlers
{
    public class UpdateCommandConfigHandler : INotificationHandler<DomainEventNotification<CommandConfigUpdatedEvent>>
    {
        private readonly ILogger<UpdateCommandConfigHandler> _logger;

        public UpdateCommandConfigHandler(ILogger<UpdateCommandConfigHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<CommandConfigUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
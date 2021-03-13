using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Roles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Roles.Handlers
{
    public class RoleCreatedEventHandler : INotificationHandler<DomainEventNotification<RoleCreatedEvent>>
    {
        private readonly ILogger<RoleCreatedEventHandler> _logger;

        public RoleCreatedEventHandler(ILogger<RoleCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<RoleCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
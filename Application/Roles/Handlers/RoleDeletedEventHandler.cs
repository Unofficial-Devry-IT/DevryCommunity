using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Roles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Roles.Handlers
{
    public class RoleDeletedEventHandler : INotificationHandler<DomainEventNotification<RoleDeletedEvent>>
    {
        private readonly ILogger<RoleDeletedEventHandler> _logger;

        public RoleDeletedEventHandler(ILogger<RoleDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<RoleDeletedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");
            
            return Task.CompletedTask;
        }
    }
}
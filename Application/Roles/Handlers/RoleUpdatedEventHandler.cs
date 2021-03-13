using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Roles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Roles.Handlers
{
    public class RoleUpdatedEventHandler : INotificationHandler<DomainEventNotification<RoleUpdatedEvent>>
    {
        private readonly ILogger<RoleUpdatedEventHandler> _logger;

        public RoleUpdatedEventHandler(ILogger<RoleUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<RoleUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            
            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
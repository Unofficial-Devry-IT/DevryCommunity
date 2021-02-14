using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.WizardConfigs.Commands;
using Domain.Events.WizardConfigs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.WizardConfigs.Handlers
{
    public class WizardConfigUpdatedEventHandler : INotificationHandler<DomainEventNotification<WizardConfigUpdatedEvent>>
    {
        private readonly ILogger<UpdateWizardConfigCommandHandler> _logger;

        public WizardConfigUpdatedEventHandler(ILogger<UpdateWizardConfigCommandHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<WizardConfigUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");
            
            return Task.CompletedTask;
        }
    }
}
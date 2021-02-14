using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Channels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Channels.Handlers
{
    public class ChannelUpdatedEventHandler : INotificationHandler<DomainEventNotification<ChannelUpdatedEvent>>
    {
        private readonly ILogger<ChannelUpdatedEventHandler> _logger;

        public ChannelUpdatedEventHandler(ILogger<ChannelUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ChannelUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
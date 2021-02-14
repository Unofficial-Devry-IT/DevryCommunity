using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Channels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Channels.Handlers
{
    public class ChannelCreatedEventHandler : INotificationHandler<DomainEventNotification<ChannelCreatedEvent>>
    {
        private readonly ILogger<ChannelCreatedEventHandler> _logger;

        public ChannelCreatedEventHandler(ILogger<ChannelCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ChannelCreatedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            
            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }

    }
}
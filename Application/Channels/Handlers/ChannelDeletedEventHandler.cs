using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Channels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Channels.Handlers
{
    public class ChannelDeletedEventHandler : INotificationHandler<DomainEventNotification<ChannelDeletedEvent>>
    {
        private readonly ILogger<ChannelDeletedEventHandler> _logger;

        public ChannelDeletedEventHandler(ILogger<ChannelDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ChannelDeletedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
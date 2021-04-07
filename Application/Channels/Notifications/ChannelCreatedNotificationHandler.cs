using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Channels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Channels.Notifications
{
    /// <summary>
    /// Taps into the <see cref="ChannelCreatedEvent"/> for notification purposes
    /// </summary>
    public class ChannelCreatedNotificationHandler : INotificationHandler<DomainEventNotification<ChannelCreatedEvent>>
    {
        private readonly ILogger<ChannelCreatedNotificationHandler> _logger;

        public ChannelCreatedNotificationHandler(ILogger<ChannelCreatedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ChannelCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Channels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Channels.Notifications
{
    /// <summary>
    /// Taps into the <see cref="ChannelUpdatedEvent"/> for logging purposes
    /// </summary>
    public class ChannelUpdatedNotificationHandler : INotificationHandler<DomainEventNotification<ChannelUpdatedEvent>>
    {
        private readonly ILogger<ChannelUpdatedNotificationHandler> _logger;

        public ChannelUpdatedNotificationHandler(ILogger<ChannelUpdatedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ChannelUpdatedEvent> notification,
            CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation($"Domain Event: {domainEvent.GetType().Name}");

            return Task.CompletedTask;
        }
    }
}
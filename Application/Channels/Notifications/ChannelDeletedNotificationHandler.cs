using System.Threading;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Events.Channels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Channels.Notifications
{
    /// <summary>
    /// Taps into the <see cref="ChannelDeletedEvent"/> event for logging purposes
    /// </summary>
    public class ChannelDeletedNotificationHandler : INotificationHandler<DomainEventNotification<ChannelDeletedEvent>>
    {
        private readonly ILogger<ChannelDeletedNotificationHandler> _logger;

        public ChannelDeletedNotificationHandler(ILogger<ChannelDeletedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<ChannelDeletedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            
            _logger.LogInformation(($"Domain Event: {domainEvent.GetType().Name}"));

            return Task.CompletedTask;
        }
    }
}
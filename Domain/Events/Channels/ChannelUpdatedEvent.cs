using Domain.Common.Models;
using Domain.Entities.Discord;

namespace Domain.Events.Channels
{
    /// <summary>
    /// Event when channel is updated
    /// </summary>
    public class ChannelUpdatedEvent : DomainEvent
    {
        /// <summary>
        /// Channel which got updated
        /// </summary>
        public Channel Channel { get; }

        public ChannelUpdatedEvent(Channel channel)
        {
            Channel = channel;
        }
    }
}
using Domain.Common.Models;
using Domain.Entities.Discord;

namespace Domain.Events.Channels
{
    /// <summary>
    /// When a channel has been created
    /// </summary>
    public class ChannelCreatedEvent : DomainEvent
    {
        /// <summary>
        /// Channel which got created
        /// </summary>
        public Channel Channel { get; }

        public ChannelCreatedEvent(Channel channel)
        {
            Channel = channel;
        }
    }
}
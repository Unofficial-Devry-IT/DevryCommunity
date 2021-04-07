using Domain.Common.Models;
using Domain.Entities.Discord;

namespace Domain.Events.Channels
{
    /// <summary>
    /// Event when channel gets deleted
    /// </summary>
    public class ChannelDeletedEvent : DomainEvent
    {
        /// <summary>
        /// Channel which got deleted
        /// </summary>
        public Channel Channel { get; }

        public ChannelDeletedEvent(Channel channel)
        {
            Channel = channel;
        }
    }
}
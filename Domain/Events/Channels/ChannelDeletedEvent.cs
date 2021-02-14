using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Channels
{
    public class ChannelDeletedEvent : DomainEvent
    {
        public Channel Channel { get; }

        public ChannelDeletedEvent(Channel channel)
        {
            Channel = channel;
        }
    }
}
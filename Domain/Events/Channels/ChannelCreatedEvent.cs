using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Channels
{
    public class ChannelCreatedEvent : DomainEvent
    {
        public ChannelCreatedEvent(Channel channel)
        {
            Channel = channel;
        }

        public Channel Channel { get; }
    }
}
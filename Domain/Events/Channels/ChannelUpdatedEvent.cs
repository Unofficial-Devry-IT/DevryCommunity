using Domain.Common;
using Domain.Entities;

namespace Domain.Events.Channels
{
    public class ChannelUpdatedEvent : DomainEvent
    {
        public Channel Channel { get; }

        public ChannelUpdatedEvent(Channel channel)
        {
            Channel = channel;
        }
    }
}
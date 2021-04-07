using Domain.Common.Models;
using MediatR;

namespace Application.Common.Models
{
    /// <summary>
    /// Notification/event which gets passed across the architecture
    /// </summary>
    /// <typeparam name="TDomainEvent">Type of domain event</typeparam>
    public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
    {
        public TDomainEvent DomainEvent { get; }

        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
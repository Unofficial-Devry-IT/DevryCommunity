using System;
using System.Collections.Generic;

namespace Domain.Common.Models
{
    /// <summary>
    /// For entities which events can originate from
    /// </summary>
    public interface IHasDomainEvent
    {
        public List<DomainEvent> DomainEvents { get; set; }
    }
    
    /// <summary>
    /// Base implementation of a Domain Event
    /// </summary>
    public abstract class DomainEvent
    {
        public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.Now;
        public bool IsPublished { get; set; }
    }
}
using System;

namespace Domain.Entities
{
    /// <summary>
    /// Primary Key is defaulted to <see cref="Guid"/>
    /// </summary>
    public class EntityBase : EntityWithTypedId<Guid>
    {
        public EntityBase()
        {
            Id = Guid.NewGuid();    
        }
        
        public override Guid Id { get; protected set; }
    }
}
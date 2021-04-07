namespace Domain.Entities
{
    /// <summary>
    /// Implementation for basic primary key
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class EntityWithTypedId<TId> : IEntityTypeWithId<TId>
    {
        public virtual TId Id { get; protected set; }
    }
}
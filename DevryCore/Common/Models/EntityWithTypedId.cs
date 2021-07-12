namespace DevryCore.Common.Models
{
    public class EntityWithTypedId<TId> : IEntityWithTypedId<TId>
    {
        public TId Id { get; set; }
    }
}
namespace DevryCore.Common.Models
{
    public interface IEntityWithTypedId<TId>
    {
        TId Id { get; }
    }
}
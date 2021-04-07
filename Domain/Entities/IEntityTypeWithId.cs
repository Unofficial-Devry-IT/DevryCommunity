namespace Domain.Entities
{
    /// <summary>
    /// Contract which indicates an entity shall have a particular type of Primary Key
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IEntityTypeWithId<TId>
    {
        TId Id { get; }
    }
}
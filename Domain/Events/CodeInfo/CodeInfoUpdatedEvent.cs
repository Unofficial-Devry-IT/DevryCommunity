namespace Domain.Events.CodeInfo
{
    public class CodeInfoUpdatedEvent : BaseEvent<Entities.CodeInfo>
    {
        public CodeInfoUpdatedEvent(Entities.CodeInfo info) : base(info)
        {
            
        }
    }
}
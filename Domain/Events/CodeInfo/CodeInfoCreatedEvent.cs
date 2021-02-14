namespace Domain.Events.CodeInfo
{
    public class CodeInfoCreatedEvent : BaseEvent<Entities.CodeInfo>
    {
        public CodeInfoCreatedEvent(Entities.CodeInfo info) : base(info)
        {
            
        }
    }
}
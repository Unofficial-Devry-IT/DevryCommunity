namespace Domain.Events.CodeInfo
{
    public class CodeInfoDeletedEvent : BaseEvent<Entities.CodeInfo>
    {
        public CodeInfoDeletedEvent(Entities.CodeInfo info) : base(info)
        {
            
        }
    }
}
using DevryCore.Common.Models;

namespace DevryDomain.Models
{
    public class MemberStats : EntityWithTypedId<ulong>
    {
        public double Points { get; set; }
    }
}
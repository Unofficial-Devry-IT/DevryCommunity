using UnofficialDevryIT.Architecture.Models;

namespace DevryDomain.Models
{
    public class GamificationCategory : EntityWithTypedId<ulong>
    {
        public string Name { get; set; }
    }
}
using UnofficialDevryIT.Architecture.Models;

namespace DevryDomain.Models
{
    public class GamificationEntry : EntityWithTypedId<ulong>
    {
        /// <summary>
        /// Current value of gamification topic the user has
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Category associated with this gamification entry
        /// </summary>
        public ulong GamificationCategoryId { get; set; }

        /// <summary>
        /// Discord Member ID
        /// </summary>
        public ulong UserId { get; set; }
    }
}
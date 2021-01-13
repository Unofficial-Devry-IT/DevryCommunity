using DevryService.Core;
using System;

namespace DevryService.Models
{
    public class MemberClassInfo : EntityWithTypedId<string>
    {
        public MemberClassInfo()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ulong UserId { get; set; }
        public ulong RoleId { get; set; }

        /// <summary>
        /// point in time user joined this role
        /// </summary>
        public DateTime? Joined { get; set; }

        /// <summary>
        /// point in time user left this role
        /// </summary>
        public DateTime? Left { get; set; }
    }
}

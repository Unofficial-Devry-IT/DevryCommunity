using System;
using DevryCore.Common.Models;

namespace DevryDomain.Models
{
    public class MemberClassInfo : EntityWithTypedId<string>
    {
        public MemberClassInfo()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ulong UserId { get; set; }
        public ulong RoleId { get; set; }

        public DateTime? Joined { get; set; }
        public DateTime? Left { get; set; }
    }
}
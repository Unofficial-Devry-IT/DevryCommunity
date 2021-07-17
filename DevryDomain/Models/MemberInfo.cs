using System;
using DevryCore.Common.Models;

namespace DevryDomain.Models
{
    public class MemberInfo : EntityWithTypedId<string>
    {
        public MemberInfo()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ulong UserId { get; set; }
        
        /// <summary>
        /// Type of information this record pertains to. Can also be referred to as the key
        /// </summary>
        public string InfoType { get; set; }
        
        /// <summary>
        /// Value, content of the information of interest
        /// </summary>
        public string Value { get; set; }
    }
}
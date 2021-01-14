using DevryService.Core;
using DevryServices.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevryService.Models
{
    public class MemberStats : EntityWithTypedId<ulong>
    {
        public double Points { get; set; }
    }
}

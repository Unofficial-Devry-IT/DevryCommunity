using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevryServices.Common.Tasks.Scheduling
{
    public interface IScheduledTask
    {
        string Name { get; set; }
        string Schedule { get; set; }
        string Id { get; set; }
        DateTime NextRunTime { get; set; }
    }
}

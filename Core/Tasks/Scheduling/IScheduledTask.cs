using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Tasks.Scheduling
{
    /// <summary>
    /// Task that should be ran at specific intervals
    /// </summary>
    public interface IScheduledTask
    {
        string Name { get; set; }
        string Schedule { get; set; }
        Guid Id { get; set; }
        DateTime NextRunTime { get; set; }
    }
}

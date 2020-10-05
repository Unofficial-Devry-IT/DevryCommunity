using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevryService.Core.Schedule
{
    public interface IScheduledTask
    {
        string Name { get; set; }
        string Schedule { get; set; }
        string Id { get; set; }
        DateTime NextRunTime { get; set; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}

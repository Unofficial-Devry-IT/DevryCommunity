using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevryServiceBot.Scheduling
{
    public interface IScheduledTask
    {
        string Name { get; }
        string Schedule { get; }
        string Id { get; }
        DateTime NextRunTime { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}

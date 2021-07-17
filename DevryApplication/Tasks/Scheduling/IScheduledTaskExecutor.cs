using System.Threading;
using System.Threading.Tasks;
using DevryDomain.Models;

namespace DevryApplication.Tasks.Scheduling
{
    public interface IScheduledTaskExecutor
    {
        Task ProcessAsync(IScheduledTask task, CancellationToken token);
    }
}
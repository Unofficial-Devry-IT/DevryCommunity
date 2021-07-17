using System;
using System.Threading;
using System.Threading.Tasks;
using DevryDomain.Models;

namespace DevryApplication.Tasks.Scheduling
{
    public delegate Task OnAddScheduledTask(IScheduledTask task);

    public delegate Task OnRemoveScheduledTask(string id);
    
    public interface ISchedulerBackgroundService
    {
        #region Events
        event OnAddScheduledTask OnAddTask;
        event OnRemoveScheduledTask OnRemoveTask;
        #endregion
        
        Task AddTask(IScheduledTask task);
        
        Task RemoveTask(string task);

        Task ProcessAsync(IScheduledTask task, CancellationToken token);
    }
}
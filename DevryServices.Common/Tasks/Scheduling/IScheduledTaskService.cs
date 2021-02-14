using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryServices.Common.Tasks.Scheduling
{
    public interface IScheduledTaskService
    {
        event Action<IScheduledTask> OnAddTask;
        event Action<IScheduledTask> OnRemoveTask;
        Task<(bool success, string errorMessage)> AddTask(object task);
        Task<(bool success, string errorMessage)> RemoveTask(object task);
        Task<(bool success, string errorMessage)> RemoveTask(string taskId);
    }
}

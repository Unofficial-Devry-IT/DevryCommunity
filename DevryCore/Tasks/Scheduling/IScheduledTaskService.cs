using System;
using System.Threading.Tasks;

namespace DevryCore.Tasks.Scheduling
{
    /// <summary>
    /// Handles all scheduled tasks within architecture
    /// </summary>
    public interface IScheduledTaskService
    {
        /// <summary>
        /// Systems within the architecture can subscribe to this event
        /// to be notified when a task gets added
        /// </summary>
        event Action<IScheduledTask> OnAddTask;
        
        /// <summary>
        /// Systems within the architecture can subscribe to this event
        /// to be notified when a task gets removed
        /// </summary>
        event Action<IScheduledTask> OnRemoveTask;
        
        /// <summary>
        /// How to add tasks of varying types to the system
        /// </summary>
        /// <param name="task"></param>
        /// <returns>Anonymous type of bool (success), and string (error message)</returns>
        Task<(bool success, string errorMessage)> AddTask(object task);
        
        /// <summary>
        /// Using <paramref name="task"/> -- remove it from the service
        /// </summary>
        /// <param name="task"></param>
        /// <returns>Anonymous type of bool (success), and string (error message)</returns>
        Task<(bool success, string errorMessage)> RemoveTask(object task);
        
        /// <summary>
        /// Remove task from service with matching id
        /// </summary>
        /// <param name="taskId">Id of task to remove</param>
        /// <returns>Anonymous type of bool (success), and string (error message)</returns>
        Task<(bool success, string errorMessage)> RemoveTask(string taskId);
    }
}
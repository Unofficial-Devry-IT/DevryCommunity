using System;
using System.Threading.Tasks;
using DevryDomain.Models;

namespace DevryApplication.Tasks.Scheduling
{
    public delegate Task OnAddModel<T>(T model);

    public delegate Task OnRemoveModel<T>(T id);
    
    /// <summary>
    /// Handles all scheduled tasks within architecture
    /// </summary>
    public interface IScheduledTaskService<TModel> where TModel : IScheduledTask
    {
        event OnAddModel<TModel> OnAdd;
        event OnRemoveModel<TModel> OnRemove;

        /// <summary>
        /// Add task to database
        /// </summary>
        /// <param name="task"></param>
        Task Add(TModel task);

        /// <summary>
        /// Remove task with <paramref name="id"/> from database
        /// </summary>
        /// <param name="id"></param>
        Task Remove(string id);
    }
}
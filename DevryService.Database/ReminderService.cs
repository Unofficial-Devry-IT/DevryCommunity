using DevryService.Database.Models;
using DevryServices.Common.Tasks.Scheduling;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Database
{
    public class ReminderService : IScheduledTaskService
    {
        /// <summary>
        /// Action that is invokved when reminder is removed
        /// </summary>
        public event Action<IScheduledTask> OnRemoveTask;

        /// <summary>
        /// Action that is invokved when reminder is added
        /// </summary>
        public event Action<IScheduledTask> OnAddTask;

        private readonly DevryDbContext _context;

        public ReminderService(DevryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all reminders in the database
        /// </summary>
        /// <returns></returns>
        public async Task<List<Reminder>> GetAllRemindersAsync() => await _context.Reminders.ToListAsync();

        /// <summary>
        /// Retrieve all reminders based on some condition
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<List<Reminder>> GetRemindersAsync(Predicate<Reminder> condition) => await _context.Reminders
            .Where(x => condition(x))
            .ToListAsync();

        /// <summary>
        /// Attempts to remove reminder from database
        /// Invokes <see cref="OnRemoveTask"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<(bool success, string errorMessage)> RemoveTask(string id)
        {
            bool success = true;
            string errorMessage = string.Empty;

            try
            {
                Reminder reminder = await _context.Reminders.FindAsync(id);

                if(reminder != null)
                {
                    _context.Reminders.Remove(reminder);
                    await _context.SaveChangesAsync();

                    OnRemoveTask?.Invoke(reminder);
                }
                else
                {
                    success = false;
                    errorMessage = "Not Found";
                }
            }
            catch(Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
            }

            return (success, errorMessage);
        }

        /// <summary>
        /// Attempts to remove a reminder from the database
        /// Invokes <see cref="OnRemoveTask"/>
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns></returns>
        public async Task<(bool success, string errorMessage)> RemoveTask(object reminder)
        {
            bool success = true;
            string errorMessage = string.Empty;

            try
            {
                Reminder temp = (Reminder)reminder;

                _context.Remove(reminder);
                await _context.SaveChangesAsync();
                OnRemoveTask?.Invoke(temp);
            }
            catch(Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
            }

            return (success, errorMessage);
        }

        /// <summary>
        /// Attempts to add a Reminder to the database
        /// Invokes <see cref="OnAddTask"/>
        /// </summary>
        /// <param name="reminder"></param>
        /// <returns></returns>
        public async Task<(bool success, string errorMessage)> AddTask(object reminder)
        {
            bool success = true;
            string errorMessage = string.Empty;

            try
            {
                Reminder temp = (Reminder)reminder;
                await _context.AddAsync(temp);
                await _context.SaveChangesAsync();
                OnAddTask?.Invoke(temp);
            }
            catch(Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
            }

            return (success, errorMessage);
        }

    }
}

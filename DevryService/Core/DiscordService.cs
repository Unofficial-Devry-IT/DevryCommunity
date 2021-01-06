using DevryService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core
{
    public class DiscordService
    {
        private readonly DevryDbContext _context;

        public const int OK = 200;
        public const int NOT_FOUND = 404;
        public const int SERVER_ERROR = 500;

        public DiscordService(DevryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Create reminder
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> CreateReminder(Reminder model)
        {
            try
            {
                await _context.Reminders.AddAsync(model);
                await _context.SaveChangesAsync();

                return OK;
            }
            catch
            {
                return SERVER_ERROR;
            }
        }

        /// <summary>
        /// Get all reminders
        /// </summary>
        /// <returns></returns>
        public async Task<List<Reminder>> GetReminders()
        {
            try
            {
                return await _context.Reminders.ToListAsync();
            }
            catch
            {
                return new List<Reminder>();
            }
        }

        /// <summary>
        /// Get reminders based on predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<Reminder>> GetReminders(Predicate<Reminder> predicate)
        {
            try
            {
                return await _context.Reminders
                    .Where(x => predicate(x))
                    .ToListAsync();
            }
            catch
            {
                return new List<Reminder>();
            }
        }

        /// <summary>
        /// Delete reminder
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteReminder(string id)
        {
            try
            {
                Reminder reminder = await _context.Reminders.FindAsync(id);
                if (reminder == null)
                    return NOT_FOUND;

                _context.Reminders.Remove(reminder);
                await _context.SaveChangesAsync();

                return OK;
            }
            catch
            {
                return SERVER_ERROR;
            }
        }
    }
}

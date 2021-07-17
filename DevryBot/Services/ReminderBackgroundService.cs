using System;
using System.Threading.Tasks;
using DevryApplication.Common.Interfaces;
using DevryApplication.Tasks.Scheduling;
using DevryDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevryBot.Services
{
    public class ReminderBackgroundService : IScheduledTaskService<Reminder>, IDisposable
    {
        public event OnAddModel<Reminder> OnAdd;
        public event OnRemoveModel<Reminder> OnRemove;

        private readonly ILogger<ReminderBackgroundService> _logger;
        private readonly IApplicationDbContext _context;

        private readonly IServiceScope _scope;
        private readonly ISchedulerBackgroundService _backgroundService;
        
        public ReminderBackgroundService(ILogger<ReminderBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _scope = serviceProvider.CreateScope();
            
            _logger = logger;
            _context = _scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            _backgroundService = SchedulerBackgroundService.Instance;
            
            OnAdd += AddMediator;
            OnRemove += RemoveMediator;
        }

        private async Task RemoveMediator(Reminder reminder)
        {
            if (_backgroundService == null)
            {
                _logger.LogError(
                    $"Unable to process removal of task --> {nameof(ISchedulerBackgroundService)} is set to null");
                return;
            }

            await _backgroundService.RemoveTask(reminder.Id.ToString());
        }

        private async Task AddMediator(Reminder reminder)
        {
            if (_backgroundService == null)
            {
                _logger.LogError(
                    $"Unable to process addition of task --> {nameof(ISchedulerBackgroundService)} is set to null");
                return;
            }

            await _backgroundService.AddTask(reminder);
        }

        public async Task Add(Reminder task)
        {
            task.NextRunTime = NCrontab.CrontabSchedule.Parse(task.Schedule).GetNextOccurrence(DateTime.Now);
            _context.Reminders.Add(task);
            await _context.SaveChangesAsync(default);

            if(OnAdd != null)
                await OnAdd.Invoke(task);
        }

        public async Task Remove(string id)
        {
            Guid guid = Guid.Parse(id);
            
            Reminder reminder = await _context.Reminders.FirstOrDefaultAsync(x => x.Id.Equals(guid));
        
            if (reminder == null)
                return;

            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync(default);
            
            if(OnRemove != null)
                await OnRemove.Invoke(reminder);
        }

        public void Dispose()
        {
            if(_scope != null)
                _scope.Dispose();
        }
    }
}
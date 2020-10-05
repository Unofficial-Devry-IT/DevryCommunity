using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevryService.Core.Schedule
{
    public class SchedulerBackgroundService : BackgroundService
    {
        public static SchedulerBackgroundService Instance;
        private readonly ILogger<SchedulerBackgroundService> _logger;
        public SchedulerBackgroundService(ILogger<SchedulerBackgroundService> logger)
        {
            _logger = logger;
            Instance = this;
        }

        private Dictionary<string, SchedulerTaskWrapper> _scheduledTasks = new Dictionary<string, SchedulerTaskWrapper>();

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedException;
        
        public void RemoveTask(string id)
        {
            if (_scheduledTasks.ContainsKey(id))
            {
                _logger.LogInformation($"Scheduler removed task: {id}");
                _scheduledTasks.Remove(id);
            }
        }

        public void AddTask(IScheduledTask task)
        {
            if (!_scheduledTasks.ContainsKey(task.Id))
            {
                _scheduledTasks.Add(task.Id, new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(task.Schedule),
                    Task = task,
                    NextRunTime = task.NextRunTime
                });

                _logger.LogInformation($"Scheduler added task: {task.Name} | {task.Schedule}");
            }
        }

        async Task initialize()
        {
            using (DevryDbContext context = new DevryDbContext())
            {
                foreach (var reminder in await context.Reminders.ToListAsync())
                {
                    AddTask(reminder);
                }
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken token)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.Now;
            var tasksThatShouldRun = _scheduledTasks.Values.Where(x => x.ShouldRun(referenceTime));

            foreach(var task in tasksThatShouldRun)
            {
                task.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await task.Task.ExecuteAsync(token);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ?? new AggregateException(ex));
                            UnobservedException?.Invoke(this, args);

                            if (!args.Observed)
                                throw;
                        }
                    },
                    token);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await initialize();

            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}

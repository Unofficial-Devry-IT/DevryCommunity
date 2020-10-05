using DevryServiceBot.Models;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevryServiceBot.Scheduling
{
    public class SchedulerBackgroundService : BackgroundService
    {
        static SchedulerBackgroundService instance;
        public static SchedulerBackgroundService Instance
        {
            get
            {
                if(instance == null)
                {
                    using (var database = new DevryDbContext())
                    {
                        List<Reminder> reminders = new List<Reminder>();
                        
                        if(database.Reminders.Count() > 0)
                            reminders = database.Reminders.ToList();

                        instance = new SchedulerBackgroundService(reminders);
                    }
                }

                return instance;
            }
        }

        private Dictionary<string, SchedulerTaskWrapper> _scheduledTasks = new Dictionary<string, SchedulerTaskWrapper>();

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedException;

        public SchedulerBackgroundService(IEnumerable<IScheduledTask> scheduledTasks)
        {
            instance = this;

            foreach(var scheduledTask in scheduledTasks)
            {
                _scheduledTasks.Add(scheduledTask.Id, new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
                    Task = scheduledTask,
                    NextRunTime = scheduledTask.NextRunTime
                });
            }

            ExecuteAsync(default(CancellationToken));
        }

        public void RemoveTask(string id)
        {
            if(_scheduledTasks.ContainsKey(id))
                _scheduledTasks.Remove(id);
        }

        public void AddTask(IScheduledTask task)
        {
            if(!_scheduledTasks.ContainsKey(task.Id))
                _scheduledTasks.Add(task.Id, new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(task.Schedule),
                    Task = task,
                    NextRunTime = task.NextRunTime
                });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.Now;
            var tasksThatShouldRun = _scheduledTasks.Values.Where(x => x.ShouldRun(referenceTime)).ToList();
            
            foreach(var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await taskThatShouldRun.Task.ExecuteAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ?? new AggregateException(ex));
                            UnobservedException?.Invoke(this, args);

                            if (!args.Observed)
                                throw;
                        }
                    }, 
                    cancellationToken);
            }
        }
    }
}

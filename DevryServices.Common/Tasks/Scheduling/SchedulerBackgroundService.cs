using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevryServices.Common.Tasks.Scheduling
{
    public class SchedulerBackgroundService : BackgroundService
    {
        private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnoservedTaskException;

        public SchedulerBackgroundService(IEnumerable<IScheduledTask> tasks)
        {
            var referenceTime = DateTime.Now;

            foreach(var task in tasks)
            {
                _scheduledTasks.Add(new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(task.Schedule),
                    Task = task,
                    NextRunTime = referenceTime
                });
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.Now;
            var tasksThatShouldRun = _scheduledTasks.Where(x => x.ShouldRun(referenceTime)).ToList();

            foreach(var task in tasksThatShouldRun)
            {
                task.Increment();

                await taskFactory.StartNew(
                    async () =>
                    {
                        try
                        {
                            await task.Task.ExecuteAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ?? new AggregateException(ex));
                            UnoservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                                throw;
                        }
                    }, cancellationToken);
            }
        }

    }
}

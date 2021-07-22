using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevryApplication.Common.Interfaces;
using DevryDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;
using NCrontab;

namespace DevryApplication.Tasks.Scheduling
{
    public class SchedulerBackgroundService : BackgroundService, ISchedulerBackgroundService
    {
        public event OnAddScheduledTask OnAddTask;
        public event OnRemoveScheduledTask OnRemoveTask;

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnoservedTaskException;

        /// <summary>
        /// Cache of tasks that are watched by this service
        /// </summary>
        protected readonly Dictionary<string, SchedulerTaskWrapper> ScheduledTasks = new();
        
        /// <summary>
        /// Files that are scheduled for deletion after a specified amount of time
        /// </summary>
        protected readonly Dictionary<string, DateTime> FileRemoval = new();
        
        private readonly ILogger<SchedulerBackgroundService> _logger;
        private readonly IApplicationDbContext _context;
        private readonly IScheduledTaskExecutor _executor;

        public static SchedulerBackgroundService Instance;

        public SchedulerBackgroundService(ILogger<SchedulerBackgroundService> logger, IServiceProvider serviceProvider)
        {
            Instance = this;
            
            _logger = logger;
            var scope = serviceProvider.CreateScope();
            _context = scope.ServiceProvider.GetService<IApplicationDbContext>();

            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.ExportedTypes.Any(x =>
                    !x.IsInterface && x.GetInterfaces().Contains(typeof(IScheduledTaskExecutor))));

            if (assembly == null)
            {
                throw new InvalidOperationException(
                    "The scheduler background service requires a class which inherits from " +
                    nameof(IScheduledTaskExecutor));
            }

            var type = assembly.ExportedTypes.FirstOrDefault(x =>
                            !x.IsInterface && x.GetInterfaces().Contains(typeof(IScheduledTaskExecutor)));

            _executor = (IScheduledTaskExecutor) Activator.CreateInstance(type);
        }

        public void ScheduleFileDelete(string filePath, DateTime deletionTime)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError($"File: {filePath} could not be scheduled for deletion because it doesn't exist...");
                return;
            }
            
            _logger.LogInformation($"Scheduling {filePath} for deleation >= {deletionTime.ToString("F")}");

            if (FileRemoval.ContainsKey(filePath))
                FileRemoval[filePath] = deletionTime;
            else
                FileRemoval.Add(filePath, deletionTime);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TimeSpan waitPeriod = TimeSpan.FromSeconds(5);

            var taskFactory = new TaskFactory(TaskScheduler.Current);

            var reminders = await _context.Reminders.ToListAsync();

            foreach (var reminder in reminders)
                await AddTask(reminder);

            // Should give ample time for things to kick off
            await Task.Delay(TimeSpan.FromSeconds(10));
            
            while (!stoppingToken.IsCancellationRequested)
            {
                var referenceTime = DateTime.Now;

                // Remove all tasks that will never run again
                var remove = ScheduledTasks.Values.Where(x => x.WillNeverRunAgain);

                if (remove.Any())
                {
                    _logger.LogInformation($"Removing the following tasks because they will never run again...\n\t" +
                                           $"{string.Join("\n\t", remove.Select(x=>$"{x.Task.Name} | {CronExpressionDescriptor.ExpressionDescriptor.GetDescription(x.Task.Schedule)}"))}");
                    
                    foreach (var task in remove)
                        await RemoveTask(task.Task.Id.ToString());
                }

                // Cleanup the files that are scheduled for deletion
                try
                {
                    var filesToRemove = FileRemoval.Where(x => referenceTime > x.Value);
                    foreach (var file in filesToRemove)
                    {
                        _logger.LogInformation($"It is time to delete file: {file.Key}");
                        File.Delete(file.Key);
                        FileRemoval.Remove(file.Key);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error with routing removal of files - {ex.Message}: \n\t" + string.Join("\n\t", FileRemoval.Keys));
                }
                    
                // Get all the tasks that should run based on reference time
                var tasksThatShouldRun = ScheduledTasks.Values.Where(x => x.ShouldRun(referenceTime));

                foreach (var task in tasksThatShouldRun)
                {
                    // Task should be incremented to the next runnable time based on cron-string
                    task.Increment();

                    await taskFactory.StartNew(
                        async () =>
                        {
                            try
                            {
                                await ProcessAsync(task.Task, stoppingToken);
                            }
                            catch (InvalidOperationException)
                            {
                                _logger.LogWarning($"Most likely --- a reminder was removed as it was being processed");
                            }
                            catch (Exception ex)
                            {
                                var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ??
                                    new AggregateException(ex));
                                UnoservedTaskException?.Invoke(this, args);

                                _logger.LogError(ex, $"Error occurred while processing scheduled task: {task.Task.Id} | {task.Task.Name} | {task.Task.Schedule}");
                                
                                if (!args.Observed)
                                    throw;
                            }
                        }, stoppingToken);
                }
                
                await Task.Delay(waitPeriod);
            }
        }
        
        public async Task AddTask(IScheduledTask task)
        {
            ScheduledTasks.Add(task.Id.ToString(), new SchedulerTaskWrapper()
            {
                Schedule = CrontabSchedule.Parse(task.Schedule),
                Task = task,
                NextRunTime = task.NextRunTime
            });
            
            _logger.LogInformation($"{nameof(SchedulerBackgroundService)} -- Adding -- {task.Name} | {task.Id} | {task.NextRunTime.ToString("F")}");
            
            if (OnAddTask != null)
                await OnAddTask(task);
        }

        public async Task RemoveTask(string task)
        {
            if (ScheduledTasks.ContainsKey(task))
                ScheduledTasks.Remove(task);

            _logger.LogInformation($"{nameof(SchedulerBackgroundService)} -- Removing -- ID: {task}");
            
            if (OnRemoveTask != null)
                await OnRemoveTask(task);
        }

        public async Task ProcessAsync(IScheduledTask task, CancellationToken token)
        {
            if (_executor != null)
                await _executor.ProcessAsync(task, token);
            else
                _logger.LogError($"Unable to execute {task.Name} -- {nameof(IScheduledTaskExecutor)} instance is null");
        }
    }
}
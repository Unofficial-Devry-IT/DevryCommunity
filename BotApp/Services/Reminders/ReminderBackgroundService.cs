using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using DevryServices.Common.Tasks.Scheduling;
using Domain.Entities;
using Domain.Exceptions;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace BotApp.Services.Reminders
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<ReminderBackgroundService> _logger;
        private readonly IApplicationDbContext _context;
        private readonly Bot _bot;
        private IServiceScope _scope;
        private readonly bool _isDisabled;

        public static ReminderBackgroundService Instance;
        
        public ReminderBackgroundService(ILogger<ReminderBackgroundService> logger, IServiceProvider serviceProvider, IConfiguration configuration, Bot bot)
        {
            Instance = this;
            _logger = logger;
            _scope = serviceProvider.CreateScope();
            _context = _scope.ServiceProvider.GetService<IApplicationDbContext>();
            _bot = bot;

            _isDisabled = configuration.GetValue<bool>("disableReminders");
        }

        private Dictionary<string, SchedulerTaskWrapper> _scheduledTasks =
            new Dictionary<string, SchedulerTaskWrapper>();

        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedException;

        public void RemoveTask(string id)
        {
            if (_scheduledTasks.ContainsKey(id))
            {
                _logger.LogInformation($"Scheduler removed task: {id}");
                _scheduledTasks.Remove(id);
            }
        }
        
        public void UpdateTask(Reminder task)
        {
            if (_scheduledTasks.ContainsKey(task.Id))
            {
                _logger.LogInformation($"Updating Task: '{task.Id}'");
                var nextRunTime = _scheduledTasks[task.Id].NextRunTime;
                
                _scheduledTasks[task.Id] = new SchedulerTaskWrapper()
                {
                    Schedule = CrontabSchedule.Parse(task.Schedule),
                    Task = task,
                    NextRunTime = nextRunTime
                };
            }
        }
        
        public void AddTask(IScheduledTask task)
        {
            if (!_scheduledTasks.ContainsKey(task.Id))
            {
                _scheduledTasks.Add(task.Id, new SchedulerTaskWrapper()
                {
                    Schedule = CrontabSchedule.Parse(task.Schedule),
                    Task = task,
                    NextRunTime = task.NextRunTime
                });

                _logger.LogInformation($"Scheduled added task: {task.Name} | {task.Schedule}");
            }
        }

        async Task Initialize()
        {
            foreach (var reminder in await _context.Reminders.ToListAsync())
            {
                AddTask(reminder);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Initialize();

            while (!stoppingToken.IsCancellationRequested)
            {
                if(!_isDisabled)
                    await ExecuteOnceAsync(stoppingToken);
                
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _scope.Dispose();
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            
            // Cached since this will be referenced quite frequently in this method
            var referenceTime = DateTime.Now;
            
            var removeList = _scheduledTasks.Where(x => x.Value.WillNeverRunAgain)
                .ToList();

            foreach (var pair in removeList)
            {
                var reminder = await _context.Reminders.FindAsync(pair.Key);

                if (reminder == null)
                {
                    _logger.LogWarning($"Unable to locate with Id: {pair.Key}");
                    _scheduledTasks.Remove(pair.Key);
                    continue;
                }
                
                _logger.LogInformation($"Reminder: '{reminder.Name}' with Id '{reminder.Id}' is being cleaned up --Determined to never run again");
                _context.Reminders.Remove(reminder);
                _scheduledTasks.Remove(pair.Key);
            }

            var tasksThatShouldRun = _scheduledTasks.Values.Where(x => x.ShouldRun(referenceTime));

            foreach (var task in tasksThatShouldRun)
            {
                task.Increment();
                
                await taskFactory.StartNew(async () =>
                {
                    try
                    {
                        if (task.Task.GetType() == typeof(Reminder))
                        {
                            Reminder reminder = (Reminder) task.Task;

                            var allChannels = await _bot.MainGuild.GetChannelsAsync();
                            
                            var channel = allChannels.FirstOrDefault(x => x.Id == reminder.ChannelId);

                            if (channel == null)
                                throw new NotFoundException(nameof(DiscordChannel), reminder.ChannelId);

                            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                                .WithDescription(reminder.Contents)
                                .WithTitle(reminder.Name)
                                .WithColor(DiscordColor.HotPink)
                                .WithFooter(
                                    CronExpressionDescriptor.ExpressionDescriptor.GetDescription(reminder.Schedule));

                            await channel.SendMessageAsync(embed: builder.Build());
                        }
                    }
                    catch (Exception ex)
                    {
                        var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ?? new AggregateException(ex));
                        UnobservedException?.Invoke(this, args);

                        if (!args.Observed)
                            throw;
                    }
                }, cancellationToken);
            }
        }
    }
}
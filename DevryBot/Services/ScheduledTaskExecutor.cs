using System;
using System.Threading;
using System.Threading.Tasks;
using DevryDomain.Models;
using DevryInfrastructure.Persistence;
using DisCatSharp.Entities;
using Microsoft.Extensions.Logging;
using UnofficialDevryIT.Architecture.Scheduler;

namespace DevryBot.Services
{
    public class ScheduledTaskExecutor : IScheduledTaskExecutor
    {
        private static readonly ulong MAIN_CHANNEL = 639647521316274187;
        private readonly IBot _bot;
        private readonly ILogger<ScheduledTaskExecutor> _logger;
        private readonly IApplicationDbContext _context;
        public ScheduledTaskExecutor(IBot bot, ILogger<ScheduledTaskExecutor> logger, IApplicationDbContext context)
        {
            _bot = bot;
            _logger = logger;
            _context = context;
        }
        
        public async Task ProcessAsync(IScheduledTask task, CancellationToken token)
        {
            if (task == null)
                return;

            DiscordChannel channel;
            string description = string.Empty;
            
            task.NextRunTime = NCrontab.CrontabSchedule.Parse(task.Schedule).GetNextOccurrence(DateTime.Now);

            if (task.GetType() == typeof(Reminder))
            {
                Reminder reminder = task as Reminder;
                channel = _bot.MainGuild.Channels[reminder.ChannelId];
                description = reminder.Contents;

                _logger.LogInformation($"Next run time for {task.Name} | {task.NextRunTime.ToString("F")}");

                // Update database entry to new 
                _context.Reminders.Update(reminder);
                await _context.SaveChangesAsync(token);
            }
            else
                channel = _bot.MainGuild.Channels[MAIN_CHANNEL];

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor("Reminder Hat")
                .WithTitle(task.Name)
                .WithDescription(description)
                .WithColor(DiscordColor.HotPink)
                .WithFooter($"Next: {task.NextRunTime.ToString("F")}");

            await _bot.MainGuild
                .Channels[channel.Id]
                .SendMessageAsync(embed: builder.Build());
        }

        public async Task<bool> Execute<T>(T task) where T : IScheduledTask
        {
            if (task == null)
                return false;

            DiscordChannel channel;
            string description = string.Empty;

            task.NextRunTime = NCrontab.CrontabSchedule.Parse(task.Schedule).GetNextOccurrence(DateTime.Now);

            switch (task)
            {
                case Reminder reminder:
                    channel = _bot.MainGuild.Channels[reminder.ChannelId];

                    _logger.LogInformation(
                        $"Next run time for {task.Name} | {task.NextRunTime.ToString("F")}");
                    break;
                default:
                    channel = _bot.MainGuild.Channels[MAIN_CHANNEL];
                    break;
            }

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor("Reminder Hat")
                .WithTitle(task.Name)
                .WithDescription(description)
                .WithColor(DiscordColor.HotPink)
                .WithFooter($"Next: {task.NextRunTime.ToString("F")}");

            await _bot.MainGuild
                .Channels[channel.Id]
                .SendMessageAsync(builder.Build());

            return true;
        }
    }
}
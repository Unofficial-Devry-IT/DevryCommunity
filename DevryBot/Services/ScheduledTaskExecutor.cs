using System;
using System.Threading;
using System.Threading.Tasks;
using DevryApplication.Tasks.Scheduling;
using DevryDomain.Models;
using DSharpPlusNextGen.Entities;
using Microsoft.Extensions.Logging;

namespace DevryBot.Services
{
    public class ScheduledTaskExecutor : IScheduledTaskExecutor
    {
        private static readonly ulong MAIN_CHANNEL = 639647521316274187;
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
                channel = Bot.Instance.MainGuild.Channels[reminder.ChannelId];
                description = reminder.Contents;
                
                Bot.Instance.Logger.LogInformation($"Next run time for {task.Name} | {task.NextRunTime.ToString("F")}");
                
                // Update database entry to new 
                Bot.Instance.Database.Reminders.Update(reminder);
                await Bot.Instance.Database.SaveChangesAsync(token);
            }
            else
                channel = Bot.Instance.MainGuild.Channels[MAIN_CHANNEL];
            
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor("Reminder Hat")
                .WithTitle(task.Name)
                .WithDescription(description)
                .WithColor(DiscordColor.HotPink)
                .WithFooter($"Next: {task.NextRunTime.ToString("F")}");

            await Bot.Instance
                .MainGuild
                .Channels[channel.Id]
                .SendMessageAsync(embed: builder.Build());
        }
    }
}
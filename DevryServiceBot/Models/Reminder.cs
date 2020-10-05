using DevryServiceBot.Scheduling;
using DSharpPlus.Entities;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevryServiceBot.Models
{
    public class Reminder : IScheduledTask
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }

        public string CronString { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }

        /// <summary>
        /// When was the last time this reminder was executed?
        /// </summary>
        public DateTime NextRunTime { get; set; }


        public override string ToString()
        {
            return $"Title: {Title}\nContents: {Contents}\nCron: {CronString}\n\tGuild: {GuildId}\n\tChannel: {ChannelId}\n\tId: {Id}";
        }


        #region Scheduler Implementation
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var channel = Bot.Discord.Guilds[GuildId].GetChannel(ChannelId);
            
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                                                .WithAuthor("Event - Reminder")
                                                .WithTitle(Title)
                                                .WithDescription(Contents)
                                                .WithColor(DiscordColor.HotPink)
                                                .WithImageUrl("https://www.flaticon.com/svg/static/icons/svg/1792/1792931.svg");

            await channel.SendMessageAsync(embed: builder.Build());
            try
            {
                using (var database = new DevryDbContext())
                {
                    this.NextRunTime = CrontabSchedule.Parse(CronString).GetNextOccurrence(DateTime.Now);
                    database.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await database.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Scheduler was unable to update Reminder: {Id} \n\tException: {ex.Message}\n\n\t{ex.InnerException?.Message}\n\n");
            }
        }

        public string Name => this.Title;
        public string Schedule => CronString;
        #endregion
    }
}

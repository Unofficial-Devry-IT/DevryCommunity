using DevryService.Core;
using DevryService.Core.Schedule;
using DSharpPlus.Entities;
using NCrontab;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevryService.Models
{
    public class Reminder : EntityWithTypedId<string>, IScheduledTask
    {
        public Reminder()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string Schedule { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }

        /// <summary>
        /// When the next run time should be
        /// </summary>
        public DateTime NextRunTime { get; set; }

        public async Task ExecuteAsync(CancellationToken token)
        {
            DiscordChannel channel = Bot.Discord.Guilds[GuildId].GetChannel(ChannelId);

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor("Reminder Hat")
                .WithTitle(Name)
                .WithDescription(Contents)
                .WithColor(DiscordColor.HotPink)
                .WithImageUrl("https://www.flaticon.com/svg/static/icons/svg/1792/1792931.svg");

            await channel.SendMessageAsync(embed: builder.Build());

            try
            {
                using (DevryDbContext database = new DevryDbContext())
                {
                    this.NextRunTime = CrontabSchedule.Parse(Schedule).GetNextOccurrence(DateTime.Now);
                    database.Entry(this).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await database.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Unable to update Reminder: {Id} | {Name} | {Schedule}\n\t{ex.Message}");
            }
        }
    }
}

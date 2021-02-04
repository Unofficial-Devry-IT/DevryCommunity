using DevryServices.Common.Models;
using DevryServices.Common.Tasks.Scheduling;
using System;

namespace DevryService.Database.Models
{
    public class Reminder : EntityBase, IScheduledTask
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
        public DateTime NextRunTime { get; set; }
    }
}

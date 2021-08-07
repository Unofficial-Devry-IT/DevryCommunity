using System;
using UnofficialDevryIT.Architecture.Models;
using UnofficialDevryIT.Architecture.Scheduler;

namespace DevryDomain.Models
{
    public class Reminder : EntityWithTypedId<ulong>, IScheduledTask
    {
        public ulong ChannelId { get; set; }
        public string Schedule { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public DateTimeOffset NextRunTime { get; set; }

    }
}
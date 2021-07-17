using System;
using System.Threading;
using System.Threading.Tasks;
using DevryCore.Common.Models;

namespace DevryDomain.Models
{
    public class Reminder : EntityWithTypedId<string>, IScheduledTask
    {
        public Reminder()
        {
            this.Id = Guid.NewGuid();
        }

        public ulong ChannelId { get; set; }
        public string Schedule { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }

        public DateTime NextRunTime { get; set; }

        public async Task ExecuteAsync(CancellationToken token = default)
        {
            
        }
    }
}
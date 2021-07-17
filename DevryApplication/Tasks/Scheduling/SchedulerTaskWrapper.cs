using System;
using DevryDomain.Models;
using NCrontab;

namespace DevryApplication.Tasks.Scheduling
{
    public class SchedulerTaskWrapper
    {
        public CrontabSchedule Schedule { get; set; }
        public IScheduledTask Task { get; set; }
        public DateTime LastRunTime { get; set; }
        public DateTime NextRunTime { get; set; }

        public void Increment()
        {
            LastRunTime = NextRunTime;
            NextRunTime = Schedule.GetNextOccurrence(DateTime.Now);
        }

        public bool ShouldRun(DateTime currentTime)
        {
            return NextRunTime < currentTime && LastRunTime != NextRunTime;
        }

        /// <summary>
        /// Will this instance ever run again?
        /// </summary>
        public bool WillNeverRunAgain => LastRunTime == NextRunTime;
    }
}
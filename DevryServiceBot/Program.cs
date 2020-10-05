using DevryServiceBot.Models;
using DevryServiceBot.Scheduling;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevryServiceBot
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        static void Test(string[] args)
        {
            // This does nothing... it's just to make EF-Migrations work
            using (var context = new DevryDbContext())
            {
            }

            Thread scheduler = new Thread(new ThreadStart(() =>
            {
                Bot bot = new Bot("NzU3MDQ3ODQ0MzMwMDEyNzMy.X2atvw.1fzjfUCZu8SepozBKAOLWy21LqE");
                _ = bot.Start();

                using (var context = new DevryDbContext())
                {
                    List<Reminder> reminders = context.Reminders.ToList();
                    SchedulerBackgroundService service = new SchedulerBackgroundService(reminders);
                }
            }));

            // Separate thread to handler our background service
            scheduler.Start();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            while(!cancellationTokenSource.IsCancellationRequested)
            {
                Task.Delay(1000);
            }
        }
    }
}

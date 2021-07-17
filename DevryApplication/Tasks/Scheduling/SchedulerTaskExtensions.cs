using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevryApplication.Tasks.Scheduling
{
    public static class SchedulerTaskExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services)
            => services.AddHostedService<SchedulerBackgroundService>();
            
    }
}
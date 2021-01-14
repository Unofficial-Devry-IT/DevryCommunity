using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryServices.Common.Tasks.Scheduling
{
    public static class SchedulerTaskExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services)
            => services.AddSingleton<IHostedService, SchedulerBackgroundService>();

        public static IServiceCollection AddScheduler(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler)
            => services.AddSingleton<IHostedService, SchedulerBackgroundService>(ServiceProvider =>
            {
                var instance = new SchedulerBackgroundService(ServiceProvider.GetServices<IScheduledTask>());
                instance.UnoservedTaskException += unobservedTaskExceptionHandler;
                return instance;
            });
    }
}

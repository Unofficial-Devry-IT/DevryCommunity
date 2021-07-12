using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevryCore.Tasks.Scheduling
{
    public static class SchedulerTaskExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services)
            => services.AddSingleton<IHostedService, SchedulerBackgroundService>();

        public static IServiceCollection AddScheduler<T>(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler) where T : SchedulerBackgroundService
        {
            services.AddSingleton<IHostedService, SchedulerBackgroundService>(ServiceProvider =>
            {
                var instance = (T)Activator.CreateInstance(typeof(T), new object[] { ServiceProvider });
                instance.UnoservedTaskException += unobservedTaskExceptionHandler;
                return instance;
            });

            return services;
        }
            
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevryApplication.Tasks.Scheduling
{
    public static class SchedulerTaskExtensions
    {
        public static IServiceCollection AddScheduler(this IServiceCollection services)
            => services.AddSingleton<SchedulerBackgroundService>();

        public static IServiceCollection AddScheduler<T>(this IServiceCollection services, EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskExceptionHandler) where T : SchedulerBackgroundService
        {
            services.AddSingleton<SchedulerBackgroundService>(ServiceProvider =>
            {
                var instance = (T)Activator.CreateInstance(typeof(T), new object[] { ServiceProvider });
                instance.UnoservedTaskException += unobservedTaskExceptionHandler;
                return instance;
            });

            return services;
        }
            
    }
}
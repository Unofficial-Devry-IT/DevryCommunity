using Microsoft.Extensions.DependencyInjection;
using UnofficialDevryIT.Architecture.Scheduler;
using UnofficialDevryIT.Architecture.Services;

namespace DevryApplication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDevryApplication(this IServiceCollection services)
        {
            services.AddHostedService<SchedulerBackgroundService>();
            services.AddSingleton<IScheduledTaskService>(x=>x.GetRequiredService<SchedulerBackgroundService>());
            
            return services;
        }
    }
}
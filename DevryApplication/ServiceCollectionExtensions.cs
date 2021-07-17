using DevryApplication.Tasks.Scheduling;
using Microsoft.Extensions.DependencyInjection;

namespace DevryApplication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDevryApplication(this IServiceCollection services)
        {
            services.AddScheduler();
            return services;
        }
    }
}
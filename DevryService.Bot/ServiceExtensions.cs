using DevryService.Bot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevryService.Bot
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBotServices(this IServiceCollection services)
        {
            services.AddTransient<ManagementService>();
            services.AddHostedService<Worker>();

            return services;
        }
    }
}
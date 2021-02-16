using Microsoft.Extensions.DependencyInjection;

namespace BotApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDiscordBot(this IServiceCollection services)
        {
            services.AddSingleton<Bot>();

            return services;
        }
    }
}
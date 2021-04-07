using System.Reflection;
using BotApp.Services;
using BotApp.Services.Reminders;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BotApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDiscordBot(this IServiceCollection services)
        {
            services.AddSingleton<Bot>();
            
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddHostedService<ReminderBackgroundService>();
            
            //services.AddTransient<DiscordRoleService>();
            return services;
        }
    }
}
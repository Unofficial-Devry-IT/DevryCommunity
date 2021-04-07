using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Add all MediatR handlers/commands/notifications 
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
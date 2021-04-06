using System.Reflection;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    using Application.Common.Interfaces;
    using Infrastructure.Services;
    using Microsoft.EntityFrameworkCore;
    
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure<Startup>(this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(Startup).Namespace));
                });
            }
            //services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddScoped<IDomainEventService, DomainEventService>();
            
            return services;
        }
    }
}
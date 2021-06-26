using System;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
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
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
                
                /*
                 User/Password are stored as user secrets in
                 */
                #if DEBUG
                    string dbUser = configuration.GetValue<string>("Database:User");
                    string dbPassword = configuration.GetValue<string>("Database:Password");
                    string host = configuration.GetValue<string>("Database:Host");
                #else
                    string dbUser = Environment.GetEnvironmentVariable("DATABASE_USER");
                    string dbPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
                    string host = Environment.GetEnvironmentVariable("DATABASE_HOST");
                #endif
                
                string dbConnectionString = configuration.GetConnectionString("DefaultConnection");
                
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseMySql(string.Format(dbConnectionString, host, dbUser, dbPassword), serverVersion, 
                            x=>x.MigrationsAssembly(typeof(Startup).Namespace))
                        .EnableDetailedErrors();
                });
            }
            
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            
            return services;
        }
    }
}
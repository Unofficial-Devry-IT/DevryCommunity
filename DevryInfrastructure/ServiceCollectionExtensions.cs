using System;
using System.IO;
using DevryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnofficialDevryIT.Architecture.Scheduler;

namespace DevryInfrastructure
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds infrastructure related code to a given application
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <typeparam name="TStartup">Used as a marker for where the Migrations folder is placed</typeparam>
        /// <returns></returns>
        public static IServiceCollection AddDevryInfrastructure<TStartup>(this IServiceCollection services,
            IConfiguration configuration) where TStartup : class
        {
            if (configuration.GetValue<bool>("Database:UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                }, ServiceLifetime.Singleton);
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                    string dataDirectory = Path.Join(baseDir, "Data");

                    // We want to ensure our folder exists
                    Directory.CreateDirectory(dataDirectory);
                    string dbPath = Path.Join(dataDirectory, "DevryCommunity.db");
                    
                    options.UseSqlite($"Data Source={dbPath}", 
                            x=>x.MigrationsAssembly(typeof(TStartup).Namespace))
                        .EnableDetailedErrors();
                }, ServiceLifetime.Singleton);
            }

            services.AddSingleton<IApplicationDbContext>(x=>x.GetRequiredService<ApplicationDbContext>());
            services.AddSingleton<IScheduleDbContext>(x=>x.GetRequiredService<ApplicationDbContext>());
            
            return services;
        }
    }
}
using System;
using DevryApplication.Common.Interfaces;
using DevryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("Database:UseInMemoryDatabase"))
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
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                    if (baseDir.Contains("bin"))
                    {
                        int index = baseDir.IndexOf("bin");
                        baseDir = baseDir.Substring(0, index);
                    }

                    options.UseSqlite($"Data Source={baseDir}data\\DevryCommunity.db", 
                            x=>x.MigrationsAssembly(typeof(TStartup).Namespace))
                        .EnableDetailedErrors();
                });
            }

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            return services;
        }
    }
}
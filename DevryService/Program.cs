using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevryService.Core;
using DevryService.Core.Schedule;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevryService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<DevryDbContext>();
                    services.AddHostedService<SchedulerBackgroundService>();
                    services.AddHostedService<Worker>();
                    services.AddTransient<DiscordService>();
                })
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    // Acquire/Build out necessary configuration files
                    var configFiles = Core.Util.ConfigHandler.InitializeSettings();
                    // Add each configuration file
                    foreach (var file in configFiles)
                        builder.AddJsonFile(file, optional: false, reloadOnChange: true);

                    builder.AddUserSecrets<Program>();
                });
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevryService.Bot
{
    public class Worker : BackgroundService
    {
        public readonly ILogger<Worker> Logger;
        public static Worker Instance;
        public static IConfiguration Configuration;

        public Bot Bot;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, ILogger<Bot> botLogger, IServiceProvider serviceProvider)
        {
            Logger = logger;
            Configuration = configuration;

            Instance = this;
            
            // Instantiate bot
            Bot = new Bot(configuration, botLogger, serviceProvider);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Bot.StartAsync();

            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, stoppingToken);
        }
    }
}

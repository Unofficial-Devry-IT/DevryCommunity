using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevryService
{
    public class Worker : BackgroundService
    {
        public readonly ILogger<Worker> Logger;
        private readonly string _token;

        public static Worker Instance;
        

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            Logger = logger;
            _token = configuration.GetValue<string>("token");

            Instance = this;

            if (string.IsNullOrEmpty(_token))
                throw new ArgumentNullException(nameof(_token));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Bot bot = new Bot(_token);
            await bot.StartAsync();

            while (!stoppingToken.IsCancellationRequested)
            {                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

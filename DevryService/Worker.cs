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
        public static Worker Instance;
        public static IConfiguration Configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;
            Instance = this;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Bot bot = new Bot(Configuration);
            await bot.StartAsync();

            while (!stoppingToken.IsCancellationRequested)
            {                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using DevryService.Core;
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
        public DiscordService DiscordService;

        Bot bot;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, ILogger<Bot> botLogger, DiscordService discordService)
        {
            Logger = logger;
            Configuration = configuration;
            DiscordService = discordService;

            Instance = this;

            bot = new Bot(configuration, botLogger, discordService);
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await bot.StartAsync();

            while (!stoppingToken.IsCancellationRequested)
            {                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

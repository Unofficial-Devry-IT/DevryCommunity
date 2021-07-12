using System.Threading;
using System.Threading.Tasks;
using DevryInfrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevryBot
{
    public class DevryBotWorker : BackgroundService
    {
        private readonly ILogger<DevryBotWorker> _logger;

        private readonly Bot _bot;
        private readonly ApplicationDbContext _context;
        
        public DevryBotWorker(ILogger<DevryBotWorker> logger, IConfiguration configuration, ILogger<Bot> botLogger)// ApplicationDbContext context)
        {
            _logger = logger;

            //_context = context;

            _bot = new Bot(configuration, botLogger); //, context);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await _context.Database.EnsureCreatedAsync(stoppingToken);
            await _bot.RunAsync();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

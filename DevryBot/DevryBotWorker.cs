using System;
using System.Threading;
using System.Threading.Tasks;
using DevryInfrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevryBot
{
    public class DevryBotWorker : BackgroundService
    {
        private readonly ILogger<DevryBotWorker> _logger;
        private readonly Bot _bot;
        private readonly ApplicationDbContext _context;
        private readonly IServiceScope _scope;
        
        public DevryBotWorker(ILogger<DevryBotWorker> logger, IConfiguration configuration, ILogger<Bot> botLogger, IServiceProvider serviceProvider)
        {
            _scope = serviceProvider.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                _context.Database.EnsureCreated();
            }
            catch
            {
            }

            _logger = logger;
            _bot = new Bot(configuration, botLogger, _context, serviceProvider);
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

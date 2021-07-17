using DevryApplication;
using DevryApplication.Tasks.Scheduling;
using DevryBot.Services;
using DevryInfrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevryBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDevryInfrastructure<Bot>(Configuration);
            services.AddDevryApplication();
            services.AddSingleton<ReminderBackgroundService>();
            services.AddHostedService<DevryBotWorker>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
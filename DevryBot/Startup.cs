﻿using System;
using ChallengeAssistant;
using DevryBot.Options;
using DevryBot.Services;
using DevryInfrastructure;
using DevryInfrastructure.Persistence;
using ImageCreator.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnofficialDevryIT.Architecture.Scheduler;
using UnofficialDevryIT.Architecture.Services;

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
            services.AddOptions();
            services.Configure<DiscordOptions>(Configuration.GetSection("Discord"));
            services.Configure<WelcomeOptions>(Configuration.GetSection("WelcomeSettings"));
            services.Configure<ArchiveOptions>(Configuration.GetSection("ArchiveSettings"));
            services.Configure<ClassCreationOptions>(Configuration.GetSection("ClassCreation"));
            services.Configure<ChallengeOptions>(Configuration.GetSection("ChallengeSettings"));
            
            StorageHandler.InitializeFolderStructure();
            
            services.AddDevryInfrastructure<Bot>(Configuration);

            if (!Configuration.GetValue<bool>("Database:UpdateDatabase"))
            {
                services.AddSingleton<IBot, Bot>();
                services.AddSingleton<IWelcomeHandler, WelcomeHandler>();
                services.AddSingleton<IScheduledTaskExecutor, ScheduledTaskExecutor>();
                services.AddSingleton<IScheduledTaskService, SchedulerBackgroundService>();
                services.AddSingleton<IImageService, UnsplashImageService>();

                services.AddChallengeApis();

                services.AddSingleton<IGamificationService, GamificationService>();


                // This is done to ensure the SAME bot is utilized from above
                services.AddHostedService(x=>(Bot)x.GetRequiredService<IBot>());
                services.AddHostedService(x => (SchedulerBackgroundService)x.GetRequiredService<IScheduledTaskService>());
                services.AddHostedService(x => (GamificationService)x.GetRequiredService<IGamificationService>());

                services.AddSingleton<IRoleService, RoleService>();    
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
        }
    }
}
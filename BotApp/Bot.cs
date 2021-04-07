using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Extensions;
using BotApp.Services;
using Domain.Entities;
using Domain.Entities.Discord;
using Domain.Events.Channels;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ChannelType = Domain.Enums.ChannelType;

namespace BotApp
{
    public class Bot : IDisposable
    {
        public static DiscordClient Discord;
        public static CommandsNextExtension Commands;
        public static InteractivityExtension Interactivity;
        public static Bot Instance;

        public string Prefix { get; private set; }
        public ILogger Logger { get; private set; }
        public IConfiguration Configuration { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public DiscordGuild MainGuild { get; private set; }
        public IApplicationDbContext Context { get; private set; }
        private readonly IServiceScope _scope;

        private readonly IConfigService _configService;
        
        public Bot(IConfiguration configuration, ILogger<Bot> logger, IServiceProvider provider)
        {
            Instance = this;
            ServiceProvider = provider;
            Logger = logger;
            Configuration = configuration;
            _scope = provider.CreateScope();
            
            Context = _scope.ServiceProvider.GetService<IApplicationDbContext>();
            
            _configService = new ConfigService();
            _configService.InitializeInteractionConfigs();
            _configService.InitializeCommandConfigs();
            
            #if DEBUG
                Prefix = "$";
                string token = Configuration.GetValue<string>("Discord:Token").FromBase64();
            #else
                Prefix = "!";
                string token = Environment.GetEnvironmentVariable("DISCORD_TOKEN").FromBase64();
            #endif
            
            LogLevel discordLogLevel = LogLevel.Information;

            switch (configuration.GetValue<string>("discordLogLevel")?.ToLower())
            {
                case "debug":
                    discordLogLevel = LogLevel.Debug;
                    break;
                case "trace":
                    discordLogLevel = LogLevel.Trace;
                    break;
                case "warning":
                    discordLogLevel = LogLevel.Warning;
                    break;
                case "none":
                    discordLogLevel = LogLevel.None;
                    break;
                default:
                    discordLogLevel = LogLevel.Information;
                    break;
                case "critical":
                    discordLogLevel = LogLevel.Critical;
                    break;
            }
            
            Discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = discordLogLevel,
                AutoReconnect = true,
                Intents = DiscordIntents.Guilds |
                          DiscordIntents.GuildMembers |
                          DiscordIntents.GuildInvites |
                          DiscordIntents.GuildMessageReactions |
                          DiscordIntents.GuildMessages |
                          DiscordIntents.GuildMessageTyping |
                          DiscordIntents.GuildEmojis 
            });

            Task.Run(StartAsync);
        }
        
        public async Task StartAsync()
        {
            Commands = Discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] {Prefix},
                EnableDms = true,
                EnableDefaultHelp = false
            });

            Interactivity = Discord.UseInteractivity(new InteractivityConfiguration()
            {
                PaginationBehaviour = DSharpPlus.Interactivity.Enums.PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromMinutes(2)
            });

            Type[] types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => (x.Name.ToLower().EndsWith("command") || x.Name.ToLower().EndsWith("commands")) &&
                            !x.IsInterface && !x.IsAbstract)
                .ToArray();

            foreach (var type in types)
            {
                Logger.LogDebug($"Registering Command: '{type.Name}'");
                //Commands.RegisterCommands(type);
            }

            Discord.GuildMemberAdded += Discord_GuildMemberAdded;
            
            
            /*
              We shall track when channels are created, updated, or deleted
              - This works two ways
                - if user did this via discord itself
                - or within the app/architecture
             */
            Discord.ChannelCreated += DiscordOnChannelCreated;
            Discord.ChannelDeleted += DiscordOnChannelDeleted;
            Discord.ChannelUpdated += DiscordOnChannelUpdated;
            
            await Discord.ConnectAsync();
            
            #if DEBUG
            MainGuild = await Discord.GetGuildAsync(642161335089627156);
            #else
            MainGuild = await Discord.GetGuildAsync(618254766396538901);
            #endif
            if (MainGuild == null)
                throw new ArgumentNullException(nameof(MainGuild));

            await SyncDiscordChannels();
        }

        private async Task DiscordOnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs e)
        {
            var entity = await Context.Channels.FindAsync(e.Channel.Id);

            if (entity == null)
                return;

            Context.Channels.Remove(entity);
            await Context.SaveChangesAsync(CancellationToken.None);
        }

        private async Task DiscordOnChannelUpdated(DiscordClient sender, ChannelUpdateEventArgs e)
        {
            var entity = await Context.Channels.FindAsync(e.ChannelAfter.Id);

            // Should never be null... but we'll ignore it if it doesn't exist within this context
            if (entity == null)
                return;

            entity.Name = e.ChannelAfter.Name;
            entity.Position = e.ChannelAfter.Position;

            await Context.SaveChangesAsync(CancellationToken.None);
        }

        /// <summary>
        /// -- We need to capture discord channels that are created manually on discord
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task DiscordOnChannelCreated(DiscordClient sender, ChannelCreateEventArgs e)
        {
            var entity = await Context.Channels.FindAsync(e.Channel.Id);

            if (entity == null)
            {
                ChannelType type = ChannelType.Text;

                switch (e.Channel.Type)
                {
                    case DSharpPlus.ChannelType.Category:
                        type = ChannelType.Category;
                        break;
                    case DSharpPlus.ChannelType.Voice:
                        type = ChannelType.Voice;
                        break;
                }
                
                entity = new Channel()
                {
                    Id = e.Channel.Id,
                    GuildId = e.Channel.GuildId.Value,
                    Name = e.Channel.Name,
                    Description = e.Channel.Topic,
                    Position = e.Channel.Position,
                    ChannelType = type
                };

                await Context.Channels.AddAsync(entity);
                await Context.SaveChangesAsync(CancellationToken.None);
            }
        }

        private Task Discord_GuildMemberAdded(DiscordClient client, GuildMemberAddEventArgs args)
        {
            try
            {
                DiscordChannel channel = MainGuild.Channels.FirstOrDefault(x =>
                    x.Value.Name.ToLower().Equals("welcome-page", StringComparison.CurrentCultureIgnoreCase)).Value;

                if (channel == null)
                    throw new ArgumentNullException("Unable to locate 'welcome-page' channel");
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred while trying to welcome '{args.Member.DisplayName}'");
            }
            
            return Task.CompletedTask;
        }

        private async Task SyncDiscordChannels()
        {
            var channels = await MainGuild.GetChannelsAsync();

            try
            {
                foreach (var channel in channels)
                {
                    var existing = await Context.Channels.FirstOrDefaultAsync(x => x.Id == channel.Id);

                    if (existing != null)
                    {
                        existing.Position = channel.Position;
                        existing.Name = channel.Name;

                        Context.Channels.Update(existing);
                    }
                    else
                    {
                        ChannelType type = ChannelType.Text;

                        switch (channel.Type)
                        {
                            case DSharpPlus.ChannelType.Category:
                                type = ChannelType.Category;
                                break;
                            case DSharpPlus.ChannelType.Voice:
                                type = ChannelType.Voice;
                                break;
                        }

                        existing = new Channel()
                        {
                            Name = channel.Name,
                            Position = channel.Position,
                            ChannelType = type,
                            GuildId = channel.GuildId.Value,
                            Id = channel.Id
                        };

                        await Context.Channels.AddAsync(existing);
                    }

                    await Context.SaveChangesAsync(default(CancellationToken));
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception.Message);
            }
        }
        
        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
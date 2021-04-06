using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
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
        public Bot(IConfiguration configuration, ILogger<Bot> logger, IServiceProvider provider)
        {
            Instance = this;
            ServiceProvider = provider;
            Logger = logger;

            _scope = provider.CreateScope();
            Context = _scope.ServiceProvider.GetService<IApplicationDbContext>();
            
            #if DEBUG
            Prefix = "$";
            #else
            Prefix = "!";
            #endif

            var bytes = Convert.FromBase64String(configuration.GetValue<string>("token"));
            string token = Encoding.UTF8.GetString(bytes).Replace("\n", "");

            Discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                AutoReconnect = true,
                Intents = DiscordIntents.Guilds |
                          DiscordIntents.GuildMembers |
                          DiscordIntents.GuildInvites |
                          DiscordIntents.GuildMessageReactions |
                          DiscordIntents.GuildMessages |
                          DiscordIntents.GuildMessageTyping |
                          DiscordIntents.GuildEmojis |
                          DiscordIntents.All
            });

            Configuration = configuration;

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
                Commands.RegisterCommands(type);
            }

            Discord.GuildMemberAdded += Discord_GuildMemberAdded;

            await Discord.ConnectAsync();
            MainGuild = await Discord.GetGuildAsync(618254766396538901);

            if (MainGuild == null)
                throw new ArgumentNullException(nameof(MainGuild));

            await SyncDiscordChannels();
        }

        private Task Discord_GuildMemberAdded(DiscordClient client,
            DSharpPlus.EventArgs.GuildMemberAddEventArgs args)
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
                            GuildId = channel.GuildId,
                            Id = channel.Id
                        };

                        await Context.Channels.AddAsync(existing);
                    }

                    await Context.SaveChangesAsync(default(CancellationToken));
                }
            }
            catch (Exception exception)
            {
                
            }
        }
        
        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BotApp
{
    public class Bot
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

        public Bot(IConfiguration configuration, ILogger<Bot> logger, IApplicationDbContext context, IServiceProvider provider)
        {
            Instance = this;
            ServiceProvider = provider;
            Logger = logger;
            Context = context;
            
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
                MinimumLogLevel = LogLevel.Information,
                AutoReconnect = true,
                Intents = DiscordIntents.Guilds |
                          DiscordIntents.GuildMembers |
                          DiscordIntents.GuildInvites |
                          DiscordIntents.GuildMessageReactions |
                          DiscordIntents.GuildMessages |
                          DiscordIntents.GuildMessageTyping |
                          DiscordIntents.GuildEmojis
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

            MainGuild = Discord.Guilds.FirstOrDefault(x => x.Value.Name.ToLower().Contains("devry")).Value;

            if (MainGuild == null)
                throw new ArgumentNullException(nameof(MainGuild));
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
    }
}
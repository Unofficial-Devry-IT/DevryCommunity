using DevryServices.Common.Helpers;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Bot
{
    public class Bot
    {
        public static DiscordClient Discord { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        public static InteractivityExtension Interactivity { get; private set; }

        public static string Prefix { get; private set; }
        
        public IConfiguration Configuration { get; private set; }
        public ILogger<Bot> Logger { get; private set; }
        
        public DiscordGuild MainGuild { get; private set; }
        public static Bot Instance;

        public IServiceProvider ServiceProvider { get; private set; }

        public Bot(IConfiguration config, ILogger<Bot> logger, IServiceProvider provider)
        {
            Instance = this;
            ServiceProvider = provider;
            Logger = logger;

#if DEBUG
            Prefix = "$";
#else
            Prefix = "!";
#endif

            var bytes = Convert.FromBase64String(config.GetValue<string>("token"));
            string token = Encoding.UTF8.GetString(bytes).Replace("\n", "");

            Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Information,
                AutoReconnect = true,
                Intents =
                        DiscordIntents.GuildEmojis |
                        DiscordIntents.GuildMembers |
                        DiscordIntents.GuildInvites |
                        DiscordIntents.GuildMessageReactions |
                        DiscordIntents.GuildMessages |
                        DiscordIntents.GuildMessageTyping |
                        DiscordIntents.Guilds
            });

            Configuration = config;            
        }

        public async Task StartAsync()
        {
            Commands = Discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { Prefix },
                EnableDms = true,
                EnableDefaultHelp = false
            });

            Interactivity = Discord.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = DSharpPlus.Interactivity.Enums.PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromMinutes(2)
            });

            Type[] types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => (x.Name.ToLower().EndsWith("command") || x.Name.ToLower().EndsWith("commands")) && !x.IsInterface && !x.IsAbstract)
                .ToArray();

            foreach(var type in types)
            {
                ConsoleHelper.Print("Registering Command: ", newLine: false);
                ConsoleHelper.Print($"'{type.Name}'", ConsoleColor.DarkYellow);

                Commands.RegisterCommands(type);
            }

            Discord.GuildMemberAdded += Discord_GuildMemberAdded;

            await Discord.ConnectAsync();
            
            MainGuild = Discord.Guilds.FirstOrDefault(x => x.Value.Name.ToLower().Contains("devry")).Value;

            if (MainGuild == null)
                throw new ArgumentNullException(nameof(MainGuild));
        }

        private async Task Discord_GuildMemberAdded(DiscordClient sender, DSharpPlus.EventArgs.GuildMemberAddEventArgs e)
        {
            try
            {
                DiscordChannel channel = MainGuild.Channels.FirstOrDefault(x => x.Value.Name.ToLower().Equals("welcome-page")).Value;

                if (channel == null)
                    throw new ArgumentNullException($"Unable to locate 'welcome-page' channel");

                //await channel.SendMessageAsync(embed: (e.Member));
            }
            catch(Exception ex)
            {
                Logger?.LogError($"An error occurred while trying to welcome '{e.Member.DisplayName}'\n\t{ex.Message}");
            }
        }
    }
}

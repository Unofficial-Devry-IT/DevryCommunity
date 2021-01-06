using DevryService.Commands;
using DevryService.Core;
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
using System.Threading.Tasks;

namespace DevryService
{
    public class Bot
    {
        public static DiscordClient Discord { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        public static InteractivityExtension Interactivity { get; private set; }
        
        public static string Prefix = "!";
        public static List<string> BlackListedRoles = new List<string>();
        public static Bot Instance;
        public IConfiguration Configuration { get; private set; }
        public ILogger<Bot> Logger { get; private set; }
        public DiscordService DiscordService { get; private set; }

        public Bot(IConfiguration config, ILogger<Bot> logger, DiscordService discordService)
        {
            Instance = this;

            this.Logger = logger;
            this.DiscordService = discordService;

            Prefix = config.GetValue<string>("prefix");
            Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = config.GetValue<string>("token"),
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                AutoReconnect = true
            });

            // Shall be used across our bot stuff
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
                PaginationBehaviour =  DSharpPlus.Interactivity.Enums.PaginationBehaviour.Ignore,
                Timeout = TimeSpan.FromMinutes(2)
            });

            Commands.RegisterCommands(typeof(Bot).Assembly);

            // Welcome message that gets dispatched to newcomers
            Discord.GuildMemberAdded += Discord_GuildMemberAdded;                       
            
            await Discord.ConnectAsync();

        }

        private async Task Discord_GuildMemberAdded(DiscordClient client, DSharpPlus.EventArgs.GuildMemberAddEventArgs e)
        {
            await e.Member.SendMessageAsync(embed: GenerateWelcomeMessage());
        }

        public static DiscordEmbed GenerateWelcomeMessage()
        {
            DiscordChannel welcomeChannel = Bot.Discord.Guilds
                .First().Value.Channels
                .FirstOrDefault(x => x.Value.Name.ToLower().Contains("welcome"))
                .Value;

            string welcomeMessage = $"Welcome to Devry IT! A community built to foster professional and educational growth! Please, introduce yourself here {welcomeChannel.Mention}.\n\n" +
                                    $"Our custom bot has a variety of features. From joining/leaving a class, viewing of code snippets, to creating channel-wide reminders! " +
                                    $"Anywhere within our server, type the following ```{Bot.Prefix}help```";

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor("Devry IT")
                .WithTitle("Welcome!")
                .WithDescription(welcomeMessage)
                .WithColor(DiscordColor.Cyan)
                .WithFooter("Note: Responding to this DM will not do anything");

            return builder.Build();
        }
    }
}

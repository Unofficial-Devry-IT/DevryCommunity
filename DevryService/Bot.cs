using DevryService.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Configuration;
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
        public static CommandsNextModule Commands { get; private set; }
        public static InteractivityModule Interactivity { get; private set; }
        
        public static string Prefix = "!";
        public static List<string> BlackListedRoles = new List<string>();

        public Bot(IConfiguration config)
        {
            Prefix = config.GetValue<string>("prefix");
            Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = config.GetValue<string>("token"),
                TokenType = TokenType.Bot,
                UseInternalLogHandler = false,
                LogLevel = LogLevel.Debug,
                AutoReconnect = true
            });

        }

        public async Task StartAsync()
        {
            Commands = Discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = Prefix,
                EnableDms = true,
                EnableDefaultHelp = false
            });

            Interactivity = Discord.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = TimeoutBehaviour.Ignore,
                PaginationTimeout = TimeSpan.FromMinutes(5),
                Timeout = TimeSpan.FromMinutes(2)
            });

            Commands.RegisterCommands<EventCommands>();
            Commands.RegisterCommands<HelpCommand>();
            Commands.RegisterCommands<SnippetCommands>();
            Commands.RegisterCommands<RoleCommands>();
            Commands.RegisterCommands<CreateClassCommand>();
            Commands.RegisterCommands<ViewCommandsCommand>();
            //Commands.RegisterCommands<ViewMembersCommand>();

            // Welcome message that gets dispatched to newcomers
            Discord.GuildMemberAdded += Discord_GuildMemberAdded;                       
            
            await Discord.ConnectAsync();
            await Discord.UpdateStatusAsync(new DiscordGame()
            {                
                Name = $"Ask for help: {Bot.Prefix}help"
            }, UserStatus.Online);

        }

        private async Task Discord_GuildMemberAdded(DSharpPlus.EventArgs.GuildMemberAddEventArgs e)
        {
            await e.Member.SendMessageAsync(embed: GenerateWelcomeMessage());
        }

        public static DiscordEmbed GenerateWelcomeMessage()
        {
            DiscordChannel welcomeChannel = Bot.Discord.Guilds
                .First().Value.Channels
                .FirstOrDefault(x => x.Name.ToLower().Contains("welcome"));

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

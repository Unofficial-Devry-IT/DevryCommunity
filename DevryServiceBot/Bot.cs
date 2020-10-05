using DevryServiceBot.Commands.General;
using DevryServiceBot.Util;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using DSharpPlus.Entities;

namespace DevryServiceBot
{
    public class Bot
    {
        public static DiscordClient Discord { get; private set; }
        public static CommandsNextModule Commands { get; private set; }
        public static InteractivityModule Interactivity { get; private set; }
        public static string Prefix = "$";

        public Bot(string token)
        {
            Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
                AutoReconnect = true,
            });            
        }

        public async Task Start()
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

            // Register ALL commands within this project (aka assembly)
            Commands.RegisterCommands<RoleCommand>();
            Commands.RegisterCommands<HelpCommands>();
            Commands.RegisterCommands<EventCommands>();
            Commands.RegisterCommands<SnippetCommand>();
            Commands.RegisterCommands<QuestionaireCommands>();

            await Discord.ConnectAsync();
            await Task.Delay(-1);
        }

        public static DiscordEmbed GenerateWelcomeMessage()
        {
            var welcomeChannel = Bot.Discord.Guilds.First().Value.Channels.FirstOrDefault(x => x.Name.ToLower().Contains("welcome"));
            string welcome_message = $"Welcome to Devry IT! A community built to foster professional and educational growth! Please, introduce yourself here {welcomeChannel.Mention}.\n\n" +
                                    $"Our custom bot has a variety of features. From joining/leaving a class, viewing of code snippets, to creating channel-wide reminders! " +
                                    $"Anywhere within our server, type the following ```{Bot.Prefix}help```";

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor("Devry IT")
                .WithTitle("Welcome!")
                .WithDescription(welcome_message)
                .WithColor(DiscordColor.Cyan)
                .WithFooter("Note: Responding to this DM will not do anything");

            return builder.Build();
        }
    }
}

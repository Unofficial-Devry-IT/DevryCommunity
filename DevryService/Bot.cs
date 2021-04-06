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

        public IConfiguration Configuration { get; private set; }
        public ILogger<Bot> Logger { get; private set; }
        public DiscordService DiscordService { get; private set; }

        public Bot(IConfiguration config, ILogger<Bot> logger, DiscordService discordService)
        {
            this.Logger = logger;
            this.DiscordService = discordService;
            Prefix = config.GetValue<string>("prefix");

#if DEBUG
            Prefix = "$";
#else
            Prefix = "!";
#endif

            var bytes = Convert.FromBase64String(config.GetValue<string>("token"));
            string token = System.Text.Encoding.UTF8.GetString(bytes).Replace("\n","");

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

            // Shall be used across our bot stuff
            Configuration = config;

            // Ensure our stuff gets loaded
            Core.Util.ConfigHandler.InitializeSettings();
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

            Type[] types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => (x.Name.ToLower().EndsWith("command") || x.Name.ToLower().EndsWith("commands")) && !x.IsInterface && !x.IsAbstract)
                .ToArray();

            foreach (var type in types)
            {
                Console.WriteLine($"Registered Command: '{type.Name}'");
                Commands.RegisterCommands(type);
            }

            // Welcome message that gets dispatched to newcomers
            Discord.GuildMemberAdded += Discord_GuildMemberAdded;

            await Discord.ConnectAsync();

        }

        private async Task Discord_GuildMemberAdded(DiscordClient client, DSharpPlus.EventArgs.GuildMemberAddEventArgs e)
        {
            try
            {
                DiscordChannel test = Bot.Discord.Guilds
                .First(x => x.Value.Name.ToLower().Contains("devry")).Value.Channels
                .FirstOrDefault(x => x.Value.Name.ToLower().Contains("welcome"))
                .Value;

                await test.SendMessageAsync(embed: GenerateWelcomeMessage(e.Member));
            }
            catch(Exception ex)
            {
                Logger?.LogError($"An error occurred while trying to welcome '{e.Member.DisplayName}'\n\t{ex.Message}");
            }
        }

        public static DiscordEmbed GenerateWelcomeMessage(DiscordMember newMember)
        {
            MessageConfig config = Core.Util.ConfigHandler.ViewWelcomeConfig();
            DiscordGuild guild = Bot.Discord.Guilds.FirstOrDefault(x => x.Value.Name.ToLower().Contains("devry")).Value;
            DiscordChannel welcomeChannel = guild.Channels
                                                    .FirstOrDefault(x => x.Value.Name.ToLower().Contains("welcome"))
                                                    .Value;

            string[] words = config.Contents.Split(" ");
            
            for(int i = 0; i < words.Length; i++)
            {
                if(words[i].StartsWith('#'))
                {
                    DiscordChannel channel = guild.Channels.FirstOrDefault(x => x.Value.Name.ToLower()
                                                                                                .Contains(words[i].ToLower()
                                                                                                                .Substring(1)))
                                                        .Value;
                    if(channel != null)
                        words[i] = channel.Mention;
                }
                else if(words[i].StartsWith("@"))
                {
                    DiscordRole role = guild.Roles
                        .Where(x => x.Value.Name.ToLower().Contains(words[i].Substring(1).ToLower()))
                        .FirstOrDefault().Value;

                    if (role != null)
                        words[i] = role.Mention;
                }
                else if(words[i].Contains("[USER]"))
                {
                    words[i] = words[i].Replace("[USER]",newMember.Mention);
                }
            }

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor("Devry IT")
                .WithTitle("Welcome!")
                .WithDescription(string.Join(" ", words))
                .WithColor(DiscordColor.Cyan);

            return builder.Build();
        }
    }
}

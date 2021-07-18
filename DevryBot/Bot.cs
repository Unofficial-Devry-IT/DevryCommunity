using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlusNextGen;
using DSharpPlusNextGen.CommandsNext;
using DSharpPlusNextGen.Entities;
using DSharpPlusNextGen.Interactivity;
using DSharpPlusNextGen.Interactivity.Enums;
using DSharpPlusNextGen.Interactivity.Extensions;
using DSharpPlusNextGen.SlashCommands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using DevryApplication.Common.Interfaces;
using DevryBot.Discord.Extensions;
using DevryBot.Discord.Interactions;
using DevryBot.Services;
using DSharpPlusNextGen.EventArgs;

namespace DevryBot
{
    public class Bot : IDisposable
    {
        #region Discord Functionality
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        public static InteractivityExtension Interactivity { get; private set; }
        #endregion
        
        /// <summary>
        /// Prefix to be utilized for invoking commands
        /// </summary>
        public static string Prefix = "!";
        
        /// <summary>
        /// If this cancellation source cancels --> shutdown bot
        /// </summary>
        public static CancellationTokenSource ShutdownRequest = new();
        
        /// <summary>
        /// Only one instance of the bot should ever be running on the same process
        /// -- Singleton approach
        /// </summary>
        public static Bot Instance;

        private DiscordGuild mainGuild;
        
        /// <summary>
        /// The primary guild should be Devry. This is cached for easier access later on
        /// </summary>
        public DiscordGuild MainGuild
        {
            get
            {
                if (mainGuild == null)
                {
                    if (Client.Guilds.Any())
                        mainGuild = Client.Guilds[618254766396538901];
                }

                return mainGuild;
            }
        }
        
        /// <summary>
        /// Bot configuration file(s) are accessible via here
        /// </summary>
        public IConfiguration Configuration { get; }
        public ILogger<Bot> Logger { get; }
        public IApplicationDbContext Database { get; }

        public Bot(IConfiguration config, ILogger<Bot> logger, IApplicationDbContext context, IServiceProvider serviceProvider)
        {
            Instance = this;
            Database = context;
            Logger = logger;
            Prefix = config.GetValue<string>("Discord:Prefix");
            
            #if DEBUG
                Prefix = "$";
            #endif
            
            var bytes = Convert.FromBase64String(config.GetValue<string>("Discord:Token"));
            string token = System.Text.Encoding.UTF8.GetString(bytes).Replace("\n", "");

            var discordConfig = new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Information,
                AutoReconnect = true,
                Intents =
                    DiscordIntents.GuildEmojisAndStickers |
                    DiscordIntents.GuildMembers |
                    DiscordIntents.GuildInvites |
                    DiscordIntents.GuildMessageReactions |
                    DiscordIntents.GuildMessages |
                    DiscordIntents.GuildMessageTyping |
                    DiscordIntents.Guilds
            };
            
            Client = new DiscordClient(discordConfig);
            
            Commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] {Prefix},
                EnableDms = true,
                EnableDefaultHelp = true,
                CaseSensitive = true,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
                DefaultHelpChecks = null
            });
            
            Commands.CommandErrored += CommandsOnCommandErrored;
            Client.MessageCreated += OnMessageCreated_RemoveDiscordLinks;
            Client.GuildMemberAdded += ClientOnGuildMemberAdded;
            Client.ComponentInteractionCreated += ClientOnComponentInteractionCreated;
            
            Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                PaginationDeletion = PaginationDeletion.DeleteMessage,
                PollBehaviour = PollBehaviour.DeleteEmojis,
                Timeout = TimeSpan.FromMinutes(1),
                PaginationEmojis = new PaginationEmojis()
                {
                    Left = DiscordEmoji.FromName(Client, ":arrow_backward:", false),
                    Right = DiscordEmoji.FromName(Client, ":arrow_forward:", false),
                    SkipLeft = DiscordEmoji.FromName(Client, ":arrow_left:", false),
                    SkipRight = DiscordEmoji.FromName(Client, ":arrow_right:", false),
                    Stop = DiscordEmoji.FromName(Client, ":stop_button:", false)
                }
            });
            
            // Register all slash commands
            SlashCommandsExtension slashCommandsExtension = Client.UseSlashCommands(new SlashCommandsConfiguration()
            {
                Services = serviceProvider
            });
            
            slashCommandsExtension.RegisterCommandsFromAssembly<Bot>();
            Configuration = config;
        }

        private async Task ClientOnComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            Logger.LogInformation($"Interaction ID: {e.Id} : {e.Message.Content} : {string.Join(", ", e.Values)}");
            var member = await e.Guild.GetMemberAsync(e.User.Id);

            // if a role based interaction was made on the welcome channel
            if (e.Id.EndsWith("_role") && e.Channel.Id == 618254766396538903)
            {

                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                    .WithTitle("Sorting Hat")
                    .WithDescription("Role successfully applied. Please note the additional channels available to you.");

                DiscordInteractionResponseBuilder interactionBuilder = new DiscordInteractionResponseBuilder()
                {
                    IsEphemeral = true
                };
                
                interactionBuilder.AddEmbed(embedBuilder.Build());
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, interactionBuilder);
                await WelcomeHandler.Instance.AddRoleToMember(member, e.Id);
                return;
            }

            if (e.Id.Equals("lecture_invite"))
            {
                await LectureInviteInteraction.HandleLectureInvite(member, e.Channel, e.Interaction);
                return;
            }

            if (e.Id.EndsWith("linvite"))
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DefferedMessageUpdate);
                await LectureInviteInteraction.HandleLectureInviteSelection(member, e.Interaction, e.Values);
            }
        }

        private async Task ClientOnGuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            WelcomeHandler.Instance.AddMember(e.Member);
        }

        private async Task OnMessageCreated_RemoveDiscordLinks(DiscordClient sender, MessageCreateEventArgs e)
        {
            // We don't care if it's our bot handing out links
            if (e.Author.IsBot)
                return;

            // If it's a moderator -- also don't care
            if (await e.Author.IsModerator())
                return;

            string[] words = e.Message.Content.ToLower().Split(" ");

            
            if (!words.Any(x => x.Contains(Configuration.GetInviteLinkSearchCriteria()) && x.Contains("http")))
                return;

            DiscordMessageBuilder messageBuilder = new();
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Oops")
                .WithDescription("Sharing links to other discords is frowned upon here. Please check with a moderator")
                .WithImageUrl(Configuration.UhOhImage())
                .WithFooter(e.Author.Username);

            messageBuilder.AddEmbed(embedBuilder.Build());
            
            await e.Message.RespondAsync(messageBuilder);

            messageBuilder = new();
            embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Snitch")
                .WithDescription($"{e.Author.Username} has attempted sharing a discord link\n\n" +
                                 $"{e.Message.Content}")
                .WithColor(DiscordColor.Gray)
                .WithFooter("Creation Date: " + e.Message.CreationTimestamp.ToString("F"));
            
            await e.Message.DeleteAsync("Contains a discord link -- only moderators are authorized to share such links");

            messageBuilder.AddEmbed(embedBuilder.Build());
            await e.Guild.Channels[851970581179662346].SendMessageAsync(messageBuilder);
        }

        private async Task CommandsOnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            Logger.LogError($"Command Failed: {e.Exception.Message}");
        }
        public async Task RunAsync()
        {
            await Client.ConnectAsync();

            /*
                Apparently there is a chance that slash-commands might end up duplicating...
                To combat duplications --- Discord:ClearCommands was introduced. It will remove all associated slash commands
                This may take a few minutes to fully process. Once completed the server can be restarted with this flag flipped to false
             */
            if (Configuration.GetValue<bool>("Discord:ClearCommands"))
            {
                await Task.Delay(TimeSpan.FromSeconds(10));

                var commands = await MainGuild.GetApplicationCommandsAsync();
                
                foreach (var command in commands)
                {
                    Logger.LogInformation($"Command: {command.Name} - {command.Id}");
                    await Client.DeleteGuildApplicationCommandAsync(MainGuild.Id, command.Id);
                }

                foreach (var command in await Client.GetGlobalApplicationCommandsAsync())
                {
                    Logger.LogInformation($"Command: {command.Name} - {command.Id}");
                    await Client.DeleteGlobalApplicationCommandAsync(command.Id);
                }
                
                Environment.Exit(0);
            }
            
            while (!ShutdownRequest.IsCancellationRequested)
                await Task.Delay(2000);

            await Task.Delay(2500);
            Dispose();
        }
        
        public void Dispose()
        {
            Interactivity = null;
            Commands = null;
            Client = null;
        }
    }
}
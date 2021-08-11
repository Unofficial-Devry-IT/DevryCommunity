using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Reflection;
using ChallengeAssistant.Interfaces;
using DevryBot.Discord;
using DevryBot.Discord.Attributes;
using DevryBot.Discord.Extensions;
using DevryBot.Options;
using DevryBot.Services;
using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Enums;
using DisCatSharp.Interactivity.Extensions;
using DisCatSharp.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DevryBot
{
    public class Bot : BackgroundService, IBot
    {
        #region Discord Functionality

        public DiscordClient Client { get; }
        public InteractivityExtension Interactivity { get; }
        public SlashCommandsExtension SlashCommands { get; }
        public IServiceProvider ServiceProvider { get; }
        private IWelcomeHandler _welcomeHandler;
        
        #endregion
        
        
        private DiscordGuild mainGuild;

        private const ulong GUILD_ID =
                #if DEBUG
                    642161335089627156
                #else
                    618254766396538901
                #endif
            ;
        
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
                        mainGuild = Client.Guilds[GUILD_ID];
                }

                return mainGuild;
            }
        }

        /// <summary>
        /// Bot configuration file(s) are accessible via here
        /// </summary>
        private readonly IConfiguration _configuration;
        private readonly ILogger<Bot> _logger;
        private readonly Dictionary<string, IInteractionHandler> _interactionHandlers = new();
        private readonly DiscordOptions _options;
        private readonly ChallengeOptions _challengeOptions;
        private readonly IChallengeParser _parser;
        
        public Bot(ILogger<Bot> logger, IConfiguration config, IServiceProvider serviceProvider, IOptions<DiscordOptions> discordOptions, IChallengeParser parser, IOptions<ChallengeOptions> challengeOptions)
        {
            _logger = logger;
            ServiceProvider = serviceProvider;
            _parser = parser;
            _challengeOptions = challengeOptions.Value;
            _options = discordOptions.Value;

            byte[] tokenBytes = Convert.FromBase64String(config.GetValue<string>("Discord:Token"));
            string token = System.Text.Encoding.UTF8.GetString(tokenBytes).Replace("\n", "");

            var discordConfig = new DiscordConfiguration()
            {
                Token = token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Information,
                AutoReconnect = true,
                Intents = 
                    # if DEBUG
                        DiscordIntents.All
                    #else
                        DiscordIntents.GuildEmojisAndStickers |
                        DiscordIntents.GuildMembers |
                        DiscordIntents.GuildInvites |
                        DiscordIntents.GuildMessageReactions |
                        DiscordIntents.GuildMessages |
                        DiscordIntents.GuildMessageTyping |
                        DiscordIntents.Guilds
                    #endif
            };
            
            Client = new DiscordClient(discordConfig);
            
            Client.MessageCreated += OnMessageCreated_RemoveDiscordLinks;
            Client.MessageCreated += CheckChallengeConfigUpload;
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
            
            slashCommandsExtension.RegisterCommandsFromAssembly<Bot>(GUILD_ID);
            _configuration = config;
        }

        private async Task CheckChallengeConfigUpload(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Author.IsBot || e.Message.Author.IsBot)
                return;
            
            if (e.Channel.Id != _challengeOptions.AddChallengeChannelId)
                return;

            if (e.Message.Attachments.Count < 1)
            {
                var response = await e.Message.RespondAsync(embed: new DiscordEmbedBuilder()
                    .WithDescription("I only want files... stop talking")
                    .WithTitle("Hall Monitor")
                    .WithColor(DiscordColor.Red));
                await Task.Delay(TimeSpan.FromSeconds(10));
                await e.Message.DeleteAsync();
                return;
            }
            
            WebClient client = new();
            List<string> responses = new();
            bool allValid = true;
            
            foreach (var file in e.Message.Attachments)
            {
                byte[] data;
                data = await client.DownloadDataTaskAsync(new Uri(file.Url));
                await using MemoryStream stream = new MemoryStream(data);
                try
                {
                    var result = await _parser.ParseFileAsync(stream, file.FileName.Split(".").Last());
                    if (result.Succeeded)
                        responses.Add($"{file.FileName} successfully parsed");
                    else
                    {
                        responses.Add($"{file.FileName} failed.\n\t{string.Join("\n", result.Errors)}");
                        allValid = false;
                    }
                }
                catch (NotSupportedException ex)
                {
                    var temp = new DiscordEmbedBuilder()
                        .WithTitle("Unsupported file type")
                        .WithDescription(ex.Message)
                        .WithColor(DiscordColor.Red);

                    await e.Channel.SendMessageAsync(temp.Build());
                    return;
                }
            }

            client?.Dispose();

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Parse Result")
                .WithColor(allValid ? DiscordColor.Green : DiscordColor.Red)
                .WithDescription(string.Join("\n", responses));
        
            await e.Channel.SendMessageAsync(embed.Build());
            await Task.Delay(500);
            await e.Message.DeleteAsync();
        }

        /// <summary>
        /// Cache all the interaction handlers within the assembly
        /// </summary>
        private void InitializeInteractionHandlers()
        {
            var types = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(x => x.IsAssignableTo(typeof(IInteractionHandler)) && !x.IsInterface && !x.IsAbstract)
                .ToList();
            
            foreach (var type in types)
            {
                var instance = (IInteractionHandler)ActivatorUtilities.CreateInstance(ServiceProvider, type);

                var attributes = type.GetCustomAttributes<InteractionNameAttribute>();

                if (!attributes.Any())
                {
                    _logger.LogWarning($"Found an interaction handler {type.Name} -- but it doesn't have the {nameof(InteractionNameAttribute)} attribute");
                    continue;
                }

                foreach(var attribute in attributes)
                    _interactionHandlers.Add(attribute.Name, instance);
            }
        }
        
        private async Task ClientOnComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            _logger.LogInformation($"Interaction ID: {e.Id} : {e.Message.Content} : {string.Join(", ", e.Values)}");
            var member = await e.Guild.GetMemberAsync(e.User.Id);
        
            if(_interactionHandlers.Count == 0)
                InitializeInteractionHandlers();

            if (_welcomeHandler == null)
                _welcomeHandler = ServiceProvider.GetRequiredService<IWelcomeHandler>();
            
            // if a role based interaction was made on the welcome channel
            if (e.Id.EndsWith(InteractionConstants.LECTURE_JOIN_ROLE) && e.Channel.Id == 618254766396538903)
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
                await _welcomeHandler.AddRoleToMember(member, e.Id);
                return;
            }

            var interactionId = e.Id.Split("_").Last();

            if (_interactionHandlers.ContainsKey(interactionId))
            {
                _logger.LogInformation($"Handling interaction id: {interactionId}");
                
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                await _interactionHandlers[interactionId].Handle(member, e);
            }

            if (_interactionHandlers.ContainsKey(e.Id))
            {
                _logger.LogInformation($"Handling interaction id: {e.Id}");
                
                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                await _interactionHandlers[e.Id].Handle(member, e);
            }
        }

        private async Task ClientOnGuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            _welcomeHandler.AddMember(e.Member);
        }

        private async Task OnMessageCreated_RemoveDiscordLinks(DiscordClient sender, MessageCreateEventArgs e)
        {
            // We don't care if it's our bot handing out links
            if (e.Author.IsBot)
                return;

            var member = await MainGuild.GetMemberAsync(e.Author.Id);
            
            // If it's a moderator -- also don't care
            if(member.Roles.Any(x=>x.Name.ToLower().Contains("moderator")))
                return;

            string[] words = e.Message.Content.ToLower().Split(" ");

            
            if (!words.Any(x => x.Contains(_configuration.GetInviteLinkSearchCriteria()) && x.Contains("http")))
                return;

            DiscordMessageBuilder messageBuilder = new();
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Oops")
                .WithDescription("Sharing links to other discords is frowned upon here. Please check with a moderator")
                .WithImageUrl(_options.UhOhImage)
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

        async Task ClearCommands()
        {
            await Task.Delay(TimeSpan.FromSeconds(20));

            var commands = await MainGuild.GetApplicationCommandsAsync();

            foreach (var command in commands)
            {
                _logger.LogInformation($"Command: {command.Name} - {command.Id} - deleting");
                await Client.DeleteGuildApplicationCommandAsync(mainGuild.Id, command.Id);
            }

            foreach (var command in await Client.GetGlobalApplicationCommandsAsync())
            {
                _logger.LogInformation($"Command: {command.Name} - {command.Id} - deleting");
                await Client.DeleteGlobalApplicationCommandAsync(command.Id);
            }

            Environment.Exit(0);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Client.ConnectAsync();

            if (_configuration.GetValue<bool>("Discord:ClearCommands"))
                await ClearCommands();
            
            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, stoppingToken);
        }
    }
}
using DevryService.Bot.Exceptions;
using DevryService.Database;
using DevryService.Database.Contracts;
using DevryService.Database.Models.Configs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryService.Bot.Wizards
{
    public abstract class WizardBase : IDiscordWizard
    {
        #region Archetype properties
        protected abstract string GetDefaultAuthorName();
        protected abstract string GetDefaultAuthorIcon();
        protected abstract string GetDefaultHeadline();
        protected abstract string GetDefaultDescription();
        protected abstract TimeSpan? GetDefaultTimeoutOverride();
        protected virtual string GetDefaultExtensionData() => null;

        protected virtual string GetExtensionData()
        {
            if (Config != null)
                return Config.ExtensionData;
            return GetDefaultExtensionData();
        }

        protected virtual string GetAuthorName()
        {
            if (Config != null)
                return Config.AuthorName;
            return GetDefaultAuthorName();
        }

        protected virtual string GetAuthorIcon()
        {
            if (Config != null)
                return Config.AuthorIcon;
            return GetDefaultAuthorIcon();
        }

        protected virtual string GetHeadline()
        {
            if (Config != null)
                return Config.Headline;
            return GetDefaultHeadline();
        }

        protected virtual string GetDescription()
        {
            if (Config != null)
                return Config.Description;
            return GetDefaultDescription();
        }

        protected virtual TimeSpan? GetTimeoutOverride()
        {
            if (Config != null)
                return Config.TimeoutOverride;
            return GetDefaultTimeoutOverride();
        }
        #endregion

        #region IDiscordWizard Contract
        public string AuthorName => GetAuthorName();
        public string AuthorIcon => GetAuthorIcon();
        public string Description => GetDescription();
        public string Headline => GetHeadline();
        public TimeSpan? TimeoutOverride => GetTimeoutOverride();
        public string ExtensionData => GetExtensionData();
        #endregion

        protected CommandContext Context;
        protected List<DiscordMessage> Messages = new List<DiscordMessage>();
        protected const string CANCEL_MESSAGE = "Type `stop` to cancel the wizard";

        protected ConfigService ConfigService { get; private set; }
        protected WizardConfig Config { get; set; }
        protected bool UserStopped;
        protected DevryDbContext Database;
        public WizardBase(CommandContext context)
        {
            if (context == null)
                return;

            Context = context;
            Messages.Add(context.Message);

            Initialize();
        }

        /// <summary>
        /// Basic initialization
        /// </summary>
        protected virtual void Initialize()
        {
            ConfigService = Bot.Instance.ServiceProvider.GetRequiredService<ConfigService>();
            Database = Bot.Instance.ServiceProvider.GetRequiredService<DevryDbContext>();
            Config = ConfigService.GetWizardConfig(AuthorName);
        }

        /// <summary>
        /// Creates the basic embed that this bot will use. Can customize it if needed
        /// </summary>
        /// <param name="isCancellable"></param>
        /// <returns></returns>
        protected DiscordEmbedBuilder BasicEmbed(bool isCancellable = true)
        {
            string footer = isCancellable ? $"{CANCEL_MESSAGE}\n\n--{Context.Member.DisplayName}" : Context.Member.DisplayName;


            return new DiscordEmbedBuilder()
                .WithAuthor(AuthorName, iconUrl: AuthorIcon)
                .WithTitle(Headline)
                .WithDescription(Description)
                .WithFooter(footer)
                .WithColor(DiscordColor.Cyan);
        }

        /// <summary>
        /// Generates an error embed message 
        /// </summary>
        /// <param name="errorMessage"></param>
        protected DiscordEmbed ErrorEmbed(string errorMessage)
        {
            return new DiscordEmbedBuilder()
                .WithAuthor("Hall Monitor")
                .WithDescription(errorMessage)
                .WithTitle("Error")
                .WithColor(DiscordColor.Red)
                .Build();
        }

        /// <summary>
        /// Builds an embed which shall be used when user stopps a wizard
        /// </summary>
        /// <returns></returns>
        protected DiscordEmbed StoppedEmbed()
        {
            return new DiscordEmbedBuilder()
                .WithAuthor(AuthorName)
                .WithDescription("Wizard has stopped")
                .WithFooter($"-- {Context.Member.DisplayName}")
                .WithColor(DiscordColor.Blurple)
                .Build();
        }

        /// <summary>
        /// Builds an embed which shall be used for Timeout purposes
        /// </summary>
        /// <returns></returns>
        protected DiscordEmbed TimeoutEmbed()
        {
            return new DiscordEmbedBuilder()
                .WithAuthor(AuthorName)
                .WithDescription("Timed out")
                .WithColor(DiscordColor.Red)
                .WithFooter($"-- {Context.Member.DisplayName}")
                .Build();
        }

        /// <summary>
        /// Responds to the user using <see cref="BasicEmbed(bool)"/> + <paramref name="text"/> as the description.
        /// </summary>
        /// <param name="text">Text that appears within the Description of an embed message</param>
        /// <param name="isCancellable">Is the user allowed to cancel during this message?</param>
        /// <param name="trackMessage">Shall this message be cleaned up at the end fo the wizard?</param>
        /// <returns></returns>
        protected async Task<DiscordMessage> SimpleReply(string text, bool isCancellable = false, bool trackMessage = true)
        {
            DiscordEmbedBuilder builder = BasicEmbed(isCancellable)
                .WithDescription(text);

            var message = await Context.RespondAsync(embed: builder.Build());

            if (trackMessage)
                Messages.Add(message);

            return message;
        }

        /// <summary>
        /// Responds to the user using the provided <paramref name="embed"/> message
        /// </summary>
        /// <param name="embed">Custom embed to display to user</param>
        /// <param name="trackMessage">Should this embed be tracked? --true means it gets deleted at the end of the user</param>
        /// <returns></returns>
        public async Task<DiscordMessage> SimpleReply(DiscordEmbed embed, bool trackMessage = false)
        {
            var message = await Context.RespondAsync(embed: embed);

            if (trackMessage)
                Messages.Add(message);

            return message;
        }

        /// <summary>
        /// Default predicate for processing discord messages
        /// Ensures that the provided message matches the user ID of the original invoking user
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool ReplyPredicate(DiscordMessage message)
        {
            if (message.Author.IsBot)
                return false;

            bool valid = true;
            
            // We will NOT allow the user to start another command if they're currently inside a wizard
            if(message.Content.StartsWith(Bot.Prefix) && message.Author.Id == Context.Member.Id)
            {
                _ = Task.Run(async () => await message.RespondAsync($"You can't use bot commands within {AuthorName} Wizard"));
                valid = false;
            }

            // If the user is trying to stop the current wizard
            else if(message.Content.ToLower().Trim() == "stop" && message.Author.Id == Context.Member.Id)
            {
                _ = Task.Run(async () => await message.RespondAsync($"{AuthorName} Wizard stopped"));
                UserStopped = true;
            }

            return valid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="embed"></param>
        /// <returns></returns>
        protected async Task<(DiscordMessage message, T value)> WithReply<T>(DiscordEmbed embed, bool trackMessage=true)
        {
            var message = await SimpleReply(embed: embed, true);
            
            if(trackMessage)
                Messages.Add(message);

            T value = default(T);
            bool valid = true;

            do
            {
                var result = await Bot.Interactivity.WaitForMessageAsync(predicate: ReplyPredicate, TimeoutOverride);

                if (result.TimedOut)
                {
                    await SimpleReply(embed: TimeoutEmbed(), false);
                    throw new StopWizardException(AuthorName);
                }
                else if (UserStopped)
                {
                    await SimpleReply(embed: StoppedEmbed(), false);
                    throw new StopWizardException(AuthorName);
                }

                try
                {
                    value = (T)Convert.ChangeType(result.Result.Content.Trim(), typeof(T));
                    return (result.Result, value);
                }
                catch
                {
                    valid = false;
                }
            } while (!valid);

            return (null, default(T));
        }

        /// <summary>
        /// Message the user should be replying to (that shall be modified with <paramref name="embed"/>)
        /// </summary>
        /// <typeparam name="T">Type of data you're expecting to get back from the user</typeparam>
        /// <param name="message">Message that shall be modified with the embed</param>
        /// <param name="embed">Embed that will appear</param>
        /// <param name="replyPredicate">Predicate that will filter out responses (default is null which uses <see cref="ReplyPredicate(DiscordMessage)"/></param>
        /// <returns><typeparamref name="T"/> value</returns>
        protected async Task<T> ReplyWithEdit<T>(DiscordMessage message, DiscordEmbed embed, Func<DiscordMessage, bool> replyPredicate = null)
        {
            if (replyPredicate == null)
                replyPredicate = ReplyPredicate;

            await message.ModifyAsync(embed: embed);
            
            T value = default(T);

            // Acquire the user's response
            var result = await Context.Message.GetNextMessageAsync(predicate: replyPredicate);

            // If timed out display timeout message
            if(result.TimedOut)
            {
                await SimpleReply(embed: TimeoutEmbed(), false);
                throw new StopWizardException(AuthorName);
            }
            else if(UserStopped)
            {
                await SimpleReply(embed: StoppedEmbed(), false);
                throw new StopWizardException(AuthorName);
            }

            // This message shall be added for tracking purposes
            Messages.Add(result.Result);

            string text = result.Result.Content.Trim();

            try
            {
                value = (T)Convert.ChangeType(text, typeof(T));
                return value;
            }
            catch
            {
                await SimpleReply(ErrorEmbed("Invalid Input"), false);
                throw new StopWizardException(AuthorName);
            }
        }

        /// <summary>
        /// Displays a message to the user in which they must react to
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">Message the user should be reacting to</param>
        /// <returns>Discord Emoji Name of reaction</returns>
        protected async Task<string> GetReactionTo<T>(DiscordMessage message)
        {
            // Get the user's interaction to the above bot message
            var result = await Bot.Interactivity.WaitForReactionAsync((args) =>
            {
                return args.User.Id == Context.Message.Author.Id && args.Message.Id == message.Id;
            });

            // If timed out display timeout message
            if (result.TimedOut)
            {
                await SimpleReply(embed: TimeoutEmbed(), false);
                throw new StopWizardException(AuthorName);
            }
            else if (UserStopped)
            {
                await SimpleReply(embed: StoppedEmbed(), false);
                throw new StopWizardException(AuthorName);
            }

            // convert the encoded emoji to proper name
            return DiscordEmoji.FromUnicode(Bot.Discord, result.Result.Emoji).GetDiscordName();
        }

        /// <summary>
        /// Default entrypoint for the wizard
        /// </summary>
        public void Run()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await ExecuteAsync();
                }
                catch { }
                finally
                {
                    await CleanupAsync();
                }
            });
        }

        /// <summary>
        /// Code that pertains to your wizard goes here for execution
        /// </summary>
        /// <returns></returns>
        protected abstract Task ExecuteAsync();

        /// <summary>
        /// Deletes messages that pertain to this wizard
        /// </summary>
        /// <returns></returns>
        public async Task CleanupAsync()
        {
            if (Messages.Count <= 0)
                return;

            if (Context.Channel.Type == DSharpPlus.ChannelType.Text)
                await Context.Channel.DeleteMessagesAsync(Messages.Where(x => x != null));
        }
    }
}

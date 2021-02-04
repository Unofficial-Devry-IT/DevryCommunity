using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections;

namespace DevryService.Core
{    
    public abstract class WizardBase<TOptions> : ISettings<TOptions>
        where TOptions : WizardConfig
    {
        protected TOptions _options = null;
        protected DiscordChannel _channel;

        /// <summary>
        /// List of messages that pertain to this wizard. They'll be cleaned up after the wizard expires
        /// </summary>
        protected List<DiscordMessage> _messages = new List<DiscordMessage>();

        protected DiscordMember _originalMember;
        protected DiscordMessage _recentMessage;

        protected bool userStopped = false;

        protected const string CANCEL_MESSAGE = "Type `stop` to cancel the wizard";
        protected ILogger<WizardBase<TOptions>> logger;
        protected CommandContext _context;

        public WizardBase(CommandContext commandContext)
        {
            if (commandContext == null)
                return;

            _context = commandContext;
            _channel = commandContext.Channel;
            _originalMember = commandContext.Member;
            
            // Add the command itself to the messages (for cleanup purposes)
            _messages.Add(commandContext.Message);

            Initialize();
        }

        /// <summary>
        /// Handles timout, and updates specified variable if successful
        /// </summary>
        /// <param name="context">Interactivity Results</param>
        /// <param name="variable">Variable that should be updated so long as the response doesn't timeout</param>
        protected void ReplyHandlerAction(InteractivityResult<DiscordMessage> context, ref string variable)
        {
            if (context.TimedOut)
                throw new StopWizardException(GetType().Name);

            variable = context.Result.Content;
        }

        /// <summary>
        /// Sends an error message to the channel in which this wizard originated on
        /// </summary>
        /// <param name="errorMessage">Text to appear</param>
        protected virtual async Task SendErrorMessage(string errorMessage)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor("Hall Monitor")
                .WithTitle("Error")
                .WithColor(DiscordColor.IndianRed)
                .WithDescription(errorMessage)
                .WithFooter(_options.AuthorName + (_context != null ? _context.Member.DisplayName : ""));

            await _channel.SendMessageAsync(embed: builder.Build());
        }

        /// <summary>
        /// Initialization effort --> Load settings via json
        /// </summary>
        protected virtual void Initialize()
        {
            // Create our empty options object
            _options = Activator.CreateInstance<TOptions>();

            try
            {
                IConfigurationSection section = Worker.Configuration.GetSection(typeof(TOptions).Name.Replace("Config", ""));
                Worker.Configuration.GetSection(typeof(TOptions).Name.Replace("Config","")).Bind(_options);            
                
                foreach(var property in typeof(TOptions).GetProperties())
                    if(typeof(IList).IsAssignableFrom(property.PropertyType) || typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                        property.SetValue(_options, section.GetSection(property.Name).Get(property.PropertyType));                

                foreach (var field in typeof(TOptions).GetFields())
                    if (typeof(IList).IsAssignableFrom(field.FieldType) || typeof(IEnumerable).IsAssignableFrom(field.FieldType))
                        field.SetValue(_options, section.GetSection(field.Name).Get(field.FieldType));

                LoadSettings(_options);
            }
            catch(Exception ex)
            {
                logger?.LogError($"Unable to load settings for {GetType().Name} --> {ex.Message}\n\tError Type: {ex.GetType().Name}\nReverting to Default Settings");
                this._options = DefaultSettings();
            }
        }

        /// <summary>
        /// Long running wizards shall be taken care of by being offloaded onto separate threads/tasks
        /// </summary>
        /// <param name="context"></param>
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
        /// <exception cref="StopWizardException">When user attempts to stop wizard</exception>
        protected abstract Task ExecuteAsync();

        /// <summary>
        /// Basic response predicate that could be used to filter messages
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool ResponsePredicate(DiscordMessage message)
        {
            if (message.Author.IsBot)
                return false;

            bool valid = true;
            _messages.Add(message);
            if(message.Content.StartsWith(Bot.Prefix))
            {
                _ = Task.Run(async () => await message.RespondAsync($"You can't use bot commands within {_options.AuthorName} Wizard"));
                valid = false;
            }
            else if(message.Content.ToLower().Trim() == "stop")
            {
                _ = Task.Run(async () => await message.RespondAsync($"{_options.AuthorName} Wizard stopped"));
                userStopped = true;
            }

            return valid;
        }

        /// <summary>
        /// Basic reaction predicate that could be used to filter reactions
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual bool ReactionPredicate(MessageReactionAddEventArgs args)
        {
            return true;
        }

        protected DiscordEmbedBuilder EmbedBuilder()
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor(_options.AuthorName, iconUrl: _options.AuthorIcon)
                .WithTitle(_options.Headline)
                .WithColor(DiscordColor.Cyan);

            return builder;
        }

        /// <summary>
        /// Basic message creation 
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="text">Text to display</param>
        /// <param name="isCancellable">Is this considered 'cancellable'</param>
        /// <returns><see cref="_recentMessage"/></returns>
        public async Task<DiscordMessage> SimpleReply(string text, bool isCancellable = false, bool trackMessage = true)
        {
            DiscordEmbedBuilder builder = EmbedBuilder()
                .WithDescription(text)
                .WithColor(DiscordColor.Cyan)
                .WithTitle(_options.Headline);

            _recentMessage = await _context.RespondAsync(embed: builder.Build());

            // If tracked --> the message will be 'cleaned up' at the end of the wizard
            if(trackMessage)
                _messages.Add(_recentMessage);

            if (isCancellable)
                builder = builder.WithFooter(CANCEL_MESSAGE);

            return _recentMessage;
        }

        public async Task<DiscordMessage> SimpleReply(DiscordEmbed embed, bool isCancellable = false, bool trackMessage = true)
        {
            _recentMessage = await _context.RespondAsync(embed: embed);

            if (trackMessage)
                _messages.Add(_recentMessage);

            return _recentMessage;
        }

        /// <summary>
        /// Executes <see cref="SimpleReply(CommandContext, string, bool)"/>, then waits for a user reply
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="text">Text to display</param>
        /// <param name="replyHandler">What shall happen when reply has been received</param>
        /// <param name="isCancellable">Is this cancellable</param>
        /// <param name="replyPredicate">Optional: filter out replies</param>
        /// <returns><see cref="_recentMessage"/></returns>
        public async Task<DiscordMessage> WithReply(string text,
                                                    Action<InteractivityResult<DiscordMessage>> replyHandler,
                                                    bool isCancellable = false,
                                                    Func<DiscordMessage,bool> replyPredicate = null)
        {            
            _recentMessage = await SimpleReply(text, isCancellable);

            if (replyPredicate == null)
                replyPredicate = ResponsePredicate;

            if(!_options.AcceptAnyUser)
            {
                var result = await _context.Message.GetNextMessageAsync(predicate: replyPredicate, timeoutOverride: _options.TimeoutOverride);
                replyHandler.Invoke(result);
            }
            else
            {
                var result = await Bot.Interactivity.WaitForMessageAsync(replyPredicate);
                replyHandler.Invoke(result);
            }

            if (userStopped)
                throw new StopWizardException(_options.AuthorName);

            return _recentMessage;
        }

        public async Task<DiscordMessage> WithReply(DiscordEmbed embed, Action<InteractivityResult<DiscordMessage>> replyHandler,
            bool isCancellable = false,
            Func<DiscordMessage, bool> replyPredicate = null)
        {
            _recentMessage = await SimpleReply(embed, isCancellable);

            if (replyPredicate == null)
                replyPredicate = ResponsePredicate;

            if(!_options.AcceptAnyUser)
            {
                var result = await _context.Message.GetNextMessageAsync(predicate: replyPredicate);
                replyHandler.Invoke(result);
            }
            else
            {
                var result = await Bot.Interactivity.WaitForMessageAsync(replyPredicate);
                replyHandler.Invoke(result);
            }

            return _recentMessage;
        }


        /// <summary>
        /// Modifies an existing message
        /// </summary>
        /// <param name="message">Message to modify</param>
        /// <param name="text">Text to update</param>
        /// <param name="add">Should the text be appended to current message's text?</param>
        /// <param name="isCancellable">Shall we display the <see cref="CANCEL_MESSAGE"/></param>
        /// <returns><see cref="_recentMessage"/></returns>
        public async Task<DiscordMessage> ReplyEdit(DiscordMessage message, string text, 
                                                    bool add = false, 
                                                    bool isCancellable = false)
        {
            if (add && message.Embeds.Count > 0)
                text = message.Embeds[0].Description + text;

            DiscordEmbedBuilder builder = EmbedBuilder()
                .WithDescription(text)
                .WithTitle(_options.Headline);

            if (isCancellable)
                builder = builder.WithFooter(CANCEL_MESSAGE + _originalMember != null ? $"\n--{_originalMember.Username}" : "");

            _recentMessage = await message.ModifyAsync(embed: builder.Build());

            return _recentMessage;
        }

        public async Task<DiscordMessage> ReplyEdit(DiscordMessage message, DiscordEmbed embed)
        {
            return await message.ModifyAsync(embed: embed);
        }

        public async Task<T> ReplyEditWithReply<T>(DiscordMessage message, DiscordEmbed embed, Func<DiscordMessage,bool> replyPredicate = null)
        {
            if (replyPredicate == null)
                replyPredicate = ResponsePredicate;

            await message.ModifyAsync(embed: embed);

            T value = default(T);

            if(!_options.AcceptAnyUser)
            {
                var result = await _context.Message.GetNextMessageAsync();

                if(result.TimedOut)
                {
                    await SimpleReply($"{_options.AuthorName} Wizard Timed Out...", false, false);
                    throw new StopWizardException(_options.AuthorName);
                }

                if (!_messages.Any(x => x.Id == result.Result.Id))
                    _messages.Add(result.Result);

                string text = result.Result.Content.Trim();

                try
                {
                    value = (T)Convert.ChangeType(text, typeof(T));
                    return value;
                }
                catch
                {
                    await SimpleReply(embed: EmbedBuilder().WithDescription("Invalid Input").WithColor(DiscordColor.Red).Build(), false,false);
                    throw new StopWizardException(_options.AuthorName);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async Task<DiscordMessage> ReplyEditWithReply(DiscordMessage message, string text, bool add=false, bool isCancellable=false,
            Action<InteractivityResult<DiscordMessage>> replyHandler = null,
            Func<DiscordMessage, bool> replyPredicate = null)
        {
            _recentMessage = await ReplyEdit(message, text, add: add, isCancellable: isCancellable);

            if (replyPredicate == null)
                replyPredicate = ResponsePredicate;

            if(!_options.AcceptAnyUser)
            {
                var result = await message.GetNextMessageAsync(predicate: replyPredicate);
                _messages.Add(result.Result);
                _recentMessage = result.Result;
                replyHandler.Invoke(result);
            }
            else
            {
                var result = await Bot.Interactivity.WaitForMessageAsync(replyPredicate);
                _messages.Add(result.Result);
                _recentMessage = result.Result;
                replyHandler.Invoke(result);
            }

            return _recentMessage;
        }

        public async Task CleanupAsync()
        {
            if (_messages.Count <= 0)
                return;

            if (_channel.Type == DSharpPlus.ChannelType.Text)
                await _channel.DeleteMessagesAsync(_messages.Where(x=>x != null));
        }

        public abstract TOptions DefaultSettings();
        public abstract CommandConfig DefaultCommandConfig();

        public virtual void LoadSettings(TOptions options)
        {
            
        }
    }
}

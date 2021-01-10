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

        protected const string CANCEL_MESSAGE = "Type `stop` to cancel the wizard";
        protected ILogger<WizardBase<TOptions>> logger;

        public WizardBase(CommandContext commandContext)
        {
            if (commandContext == null)
                return;

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
                .WithDescription(errorMessage);

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
                    if(property.PropertyType == typeof(List<string>) || property.PropertyType == typeof(string[]))
                        property.SetValue(_options, section.GetSection(property.Name).Get(property.PropertyType));

                foreach (var field in typeof(TOptions).GetFields())
                    if (field.FieldType == typeof(List<string>) || field.FieldType == typeof(string[]))
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
        public void Run(CommandContext context)
        {
            _ = Task.Run(async () => 
                {
                    try
                    {
                        await ExecuteAsync(context);
                    }                    
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
        protected abstract Task ExecuteAsync(CommandContext context);

        /// <summary>
        /// Basic response predicate that could be used to filter messages
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool ResponsePredicate(DiscordMessage message)
        {
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
                throw new StopWizardException(_options.AuthorName);
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
        public async Task<DiscordMessage> SimpleReply(CommandContext context, string text, bool isCancellable = false, bool trackMessage = true)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();

            if (!string.IsNullOrEmpty(_options.AuthorName))
                builder = builder.WithAuthor(name: _options.AuthorName, iconUrl: !string.IsNullOrEmpty(_options.AuthorIcon) ? _options.AuthorIcon : "");
            else if (!string.IsNullOrEmpty(_options.AuthorIcon))
                builder = builder.WithAuthor(iconUrl: _options.AuthorIcon);

            builder.Title = _options.Headline;
            builder.Description = text;
            builder.Color = DiscordColor.Cyan;

            _recentMessage = await context.RespondAsync(embed: builder.Build());

            // If tracked --> the message will be 'cleaned up' at the end of the wizard
            if(trackMessage)
                _messages.Add(_recentMessage);

            if (isCancellable)
                builder = builder.WithFooter(CANCEL_MESSAGE);

            return _recentMessage;
        }

        public async Task<DiscordMessage> SimpleReply(CommandContext context, DiscordEmbed embed, bool isCancellable = false, bool trackMessage = true)
        {
            _recentMessage = await context.RespondAsync(embed: embed);

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
        public async Task<DiscordMessage> WithReply(CommandContext context,
                                                         string text,
                                                         Action<InteractivityResult<DiscordMessage>> replyHandler,
                                                         bool isCancellable = false,
                                                         Func<DiscordMessage,bool> replyPredicate = null)
        {            
            _recentMessage = await SimpleReply(context, text, isCancellable);

            if (replyPredicate == null)
                replyPredicate = ResponsePredicate;

            if(!_options.AcceptAnyUser)
            {
                var result = await _recentMessage.GetNextMessageAsync(predicate: replyPredicate, timeoutOverride: _options.TimeoutOverride);
                replyHandler.Invoke(result);
            }
            else
            {
                var result = await Bot.Interactivity.WaitForMessageAsync(replyPredicate);
                replyHandler.Invoke(result);
            }

            return _recentMessage;
        }

        public async Task<DiscordMessage> WithReply(CommandContext context, DiscordEmbed embed, Action<InteractivityResult<DiscordMessage>> replyHandler,
            bool isCancellable = false,
            Func<DiscordMessage, bool> replyPredicate = null)
        {
            _recentMessage = await SimpleReply(context, embed, isCancellable);

            if (replyPredicate == null)
                replyPredicate = ResponsePredicate;

            if(!_options.AcceptAnyUser)
            {
                var result = await _recentMessage.GetNextMessageAsync(predicate: replyPredicate);
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
        /// Executes <see cref="SimpleReply(CommandContext, string, bool)"/> then waits for user reaction
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="text">Text to display</param>
        /// <param name="reactionHandler">What shall happen when user replies</param>
        /// <param name="isCancellable">Is this cancellable</param>
        /// <param name="reactionPredicate">Optional - filter out reactions</param>
        /// <returns><see cref="_recentMessage"/></returns>
        public async Task<DiscordMessage> WithReaction(CommandContext context,
                                                            string text,
                                                            Action<InteractivityResult<MessageReactionAddEventArgs>> reactionHandler,
                                                            bool isCancellable = false,
                                                            Func<MessageReactionAddEventArgs, bool> reactionPredicate = null)
        {
            _recentMessage = await SimpleReply(context, text, isCancellable);

            if (reactionPredicate == null)
                reactionPredicate = ReactionPredicate;

            if(!_options.AcceptAnyUser)
            {
                var result = await _recentMessage.WaitForReactionAsync(_originalMember);
                reactionHandler.Invoke(result);
            }
            else
            {
                var result = await Bot.Interactivity.WaitForReactionAsync(reactionPredicate, message: _recentMessage, user: null);
                reactionHandler.Invoke(result);
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

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor(name: _options.AuthorName, iconUrl: _options.AuthorIcon);

            if (isCancellable)
                builder = builder.WithFooter(CANCEL_MESSAGE + _originalMember != null ? $"\n--{_originalMember.Username}" : "");

            builder.Title = _options.Headline;
            builder.Description = text;
            builder.Color = DiscordColor.Cyan;

            _recentMessage = await message.ModifyAsync(embed: builder.Build());

            return _recentMessage;
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
                var result = await _recentMessage.GetNextMessageAsync(predicate: replyPredicate);
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

        public async Task<DiscordMessage> ReplyWithReaction(DiscordMessage message, 
                            string text, 
                            Action<InteractivityResult<MessageReactionAddEventArgs>> reactionHandler,
                            bool add=false, 
                            bool isCancellable=false,
                            Func<MessageReactionAddEventArgs, bool> reactionPredicate = null)
        {
            _recentMessage = await ReplyEdit(message, text, add, isCancellable);

            if (reactionPredicate == null)
                reactionPredicate = ReactionPredicate;

            if(!_options.AcceptAnyUser)
            {
                var result = await _recentMessage.WaitForReactionAsync(_originalMember);
                reactionHandler.Invoke(result);
            }
            else
            {
                var result = await Bot.Interactivity.WaitForReactionAsync(reactionPredicate, message: _recentMessage, user: null);
                reactionHandler.Invoke(result);
            }

            return _recentMessage;
        }

        public async Task CleanupAsync()
        {
            if (_messages.Count <= 0)
                return;

            if (_channel.Type == DSharpPlus.ChannelType.Text)
                await _channel.DeleteMessagesAsync(_messages);
        }

        public abstract TOptions DefaultSettings();
        public abstract CommandConfig DefaultCommandConfig();

        public virtual void LoadSettings(TOptions options)
        {
            
        }
    }
}

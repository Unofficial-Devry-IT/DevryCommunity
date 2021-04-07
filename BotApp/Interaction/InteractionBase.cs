using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotApp.Common.Exceptions;
using Domain.Entities;
using Domain.Entities.ConfigTypes;
using Domain.Enums;
using Domain.Exceptions;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ChannelType = DSharpPlus.ChannelType;

namespace BotApp.Interaction
{
    public abstract class InteractionBase
    {
        /// <summary>
        /// Config containing all settings pertaining to this instance
        /// </summary>
        public InteractionConfig CurrentConfig { get; set; }
        
        protected CommandContext Context { get; private set; }
        
        private ISender _mediator;

        /// <summary>
        /// Utilized to propagate events across the architecture / platform
        /// </summary>
        protected ISender Mediator
        {
            get => _mediator ??= Bot.Instance.ServiceProvider.GetService<ISender>();
        }
        
        /// <summary>
        /// User which invoked this
        /// </summary>
        protected string InvokingUser => Context.Message.Author.Username ?? string.Empty;

        /// <summary>
        /// Messages tracked by this wizard that will be cleaned up at the end
        /// </summary>
        protected List<DiscordMessage> TrackedMessages;
        
        /// <summary>
        /// Must uniquely identify this wizard
        /// </summary>
        /// <returns></returns>
        public virtual string InteractionName() => CurrentConfig?.AuthorName ?? "Name not provided in config";

        public InteractionBase(CommandContext context)
        {
            Context = context;
            TrackedMessages = new List<DiscordMessage>();
        }

        /// <summary>
        /// Basic Implementation of starting up the interaction
        /// </summary>
        public async Task Run()
        {
            try
            {
                CurrentConfig = await FindConfig();
                await ExecuteAsync();
            }
            // When interaction has been stopped by either the user or timeout
            catch (StopInteractionException)
            {
                await Context.ReplyWithStatus(StatusCode.WARNING, $"{InteractionName()} has timed out...", InvokingUser);
            }
            // When configuration could not be located
            catch (NotFoundException)
            {
                Bot.Instance.Logger.LogError($"Was unable to locate configuration for {GetType().Name}");
                await Context.ReplyWithStatus(StatusCode.ERROR,
                    $"Was unable to locate configuration for {InteractionName()}", InvokingUser);
            }
            // Something else we're not tracking has happened - log it
            catch (Exception ex)
            {
                Bot.Instance.Logger.LogError(ex, "Error processing interaction");
                await Context.ReplyWithStatus(StatusCode.ERROR,
                    $"A generic exception was thrown while processing your request. Perhaps try again? If the issue persists please open a ticket at {Constants.GITHUB_ISSUES}!",
                    InvokingUser);
            }
            // Once everything has been done -- cleanup all messages pertaining to this interaction chain
            finally
            {
                await CleanupAsync();
            }
        }
        
        /// <summary>
        /// Attempts to locate this instances' config in the database <br/>
        /// Uses <see cref="InteractionName"/> to locate
        /// </summary>
        /// <returns>Config as seen in the database</returns>
        /// <exception cref="NotFoundException">When config could not be located in the database</exception>
        private async Task<InteractionConfig> FindConfig()
        {
            // Check to see if we can find the configuration name within the database
            // Furthermore it must be of type INTERACTION
            var config = await Bot.Instance.Context.Configs
                .FirstOrDefaultAsync(x => x.ConfigName.Equals(InteractionName(), StringComparison.CurrentCultureIgnoreCase) && x.ConfigType == ConfigType.INTERACTION);

            if (config == null)
                throw new NotFoundException(nameof(Config),
                    $"Could not locate config for wizard '{InteractionName()}'");

            return JsonConvert.DeserializeObject<InteractionConfig>(config.ExtensionData);
        }

        /// <summary>
        /// Specific logic for your interaction goes in here
        /// </summary>
        /// <remarks>
        /// Remember - <see cref="CommandContext"/> is available via <see cref="InteractionBase"/>
        /// </remarks>
        /// <returns></returns>
        protected abstract Task ExecuteAsync();
        
        /// <summary>
        /// Deletes messages pertaining to this wizard
        /// </summary>
        /// <returns></returns>
        private async Task CleanupAsync()
        {
            // If there are any messages tracked by this instance -- we shall delete them
            if (TrackedMessages.Any())
            {
                if (Context.Channel.Type == ChannelType.Text)
                    await Context.Channel.DeleteMessagesAsync(TrackedMessages.Where(x => x != null));
            }
        }
        
        /// <summary>
        /// Attempts to retrieve a certain data type from the user via reply
        /// </summary>
        /// <param name="embed"></param>
        /// <param name="cleanupPromptAfter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected async Task<T> RetrieveData<T>(DiscordEmbed embed, bool cleanupPromptAfter = false)
        {
            var message = await Context.RespondAsync(embed);
            TrackedMessages.Add(message);

            var response = await Context.GetUserReply<T>(CurrentConfig.TimeoutOverride);

            if (cleanupPromptAfter)
            {
                await Context.Channel.DeleteMessageAsync(message);
                TrackedMessages.RemoveAt(TrackedMessages.Count - 1);
            }

            TrackedMessages.Add(response.UserMessage);
            return response.Value;
        }

        /// <summary>
        /// In the event multiple embeds are needed (paginated message)
        /// -- retrieve input type of <typeparamref name="T"/>
        /// </summary>
        /// <param name="embeds">Paginated embeds for user to view</param>
        /// <param name="cleanupAfterPrompt">Once the user completes the response -- delete messages</param>
        /// <typeparam name="T">Type of data we want from the user</typeparam>
        /// <returns></returns>
        protected async Task<T> RetrieveData<T>(DiscordEmbedBuilder[] embeds, bool cleanupAfterPrompt = false)
        {
            List<DiscordMessage> localTrack = new List<DiscordMessage>();
            
            foreach (var embed in embeds)
            {
                var message = await Context.RespondAsync(embed.Build());
                if (cleanupAfterPrompt)
                    localTrack.Add(message);
                else
                    TrackedMessages.Add(message);
            }

            var response = await Context.GetUserReply<T>(CurrentConfig.TimeoutOverride);

            if (cleanupAfterPrompt)
                await Context.Channel.DeleteMessagesAsync(localTrack);

            TrackedMessages.Add(response.UserMessage);
            return response.Value;
        }
        
        /// <summary>
        /// Alternative to <see cref="RetrieveData{T}(DSharpPlus.Entities.DiscordEmbed,bool)"/>
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="cleanupPromptAfter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected async Task<T> RetrieveData<T>(string prompt, bool cleanupPromptAfter = false)
        {
            var embed = CurrentConfig.BuildEmbed(InvokingUser)
                .WithDescription(prompt);

            var message = await Context.RespondAsync(embed: embed);
            TrackedMessages.Add(message);
            
            var response = await Context.GetUserReply<T>(CurrentConfig.TimeoutOverride);

            if (cleanupPromptAfter)
            {
                await Context.Channel.DeleteMessageAsync(message);
                TrackedMessages.RemoveAt(TrackedMessages.Count - 1);
            }
            
            TrackedMessages.Add(response.UserMessage);

            return response.Value;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotApp.Common.Exceptions;
using Domain.Entities.Configs;
using Domain.Entities.Configs.ConfigTypes;
using Domain.Enums;
using Domain.Exceptions;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ChannelType = DSharpPlus.ChannelType;

namespace BotApp.Interaction
{
    public abstract class WizardBase
    {
        public WizardConfig CurrentConfig { get; set; }

        protected CommandContext Context { get; private set; }

        private ISender _mediator;

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
        public abstract string WizardName();
        

        public WizardBase(CommandContext context)
        {
            Context = context;
            TrackedMessages = new List<DiscordMessage>();

        }

        public async Task Run()
        {
            await Task.Run(async () =>
            {
                try
                {
                    CurrentConfig = await FindConfig();
                    await ExecuteAsync(Context);
                }
                catch (StopWizardException)
                {
                    await Context.ReplyWithStatus(StatusCode.WARNING, $"{WizardName()} has timed out...", InvokingUser);
                }
                catch (NotFoundException)
                {
                    await Context.ReplyWithStatus(StatusCode.ERROR,
                        $"Was unable to locate configuration for {WizardName()}", InvokingUser);
                }
                catch (Exception ex)
                {
                    await Context.ReplyWithStatus(StatusCode.ERROR,
                        $"A generic exception was thrown while processing your request. Perhaps try again? If the issue persists please open a ticket at {Constants.GITHUB_ISSUES}!",
                        InvokingUser);
                }
                finally
                {
                    await CleanupAsync();
                }
            });
        }
        
        /// <summary>
        /// Attempts to locate this instances' config in the database <br/>
        /// Uses <see cref="WizardName"/> to locate
        /// </summary>
        /// <returns>Config as seen in the database</returns>
        /// <exception cref="NotFoundException">When config could not be located in the database</exception>
        private async Task<WizardConfig> FindConfig()
        {
            var config = await Bot.Instance.Context.Configs
                .FirstOrDefaultAsync(x => x.ConfigName.Equals(WizardName(), StringComparison.CurrentCultureIgnoreCase));

            if (config == null)
                throw new NotFoundException(nameof(Config),
                    $"Could not locate config for wizard '{WizardName()}'");

            return JsonConvert.DeserializeObject<WizardConfig>(config.ExtensionData);
        }

        protected abstract Task ExecuteAsync(CommandContext context);
        
        /// <summary>
        /// Deletes messages pertaining to this wizard
        /// </summary>
        /// <returns></returns>
        private async Task CleanupAsync()
        {
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
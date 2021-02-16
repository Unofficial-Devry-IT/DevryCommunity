using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotApp.Common.Exceptions;
using Domain.Entities.Configs;
using Domain.Enums;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;

namespace BotApp.Interaction
{
    public static class BaseInteractions
    {
        public struct MessageResult<T>
        {
            public T Value { get; set; }
            public DiscordMessage UserMessage { get; set; }
        }

        public static bool DefaultPredicate(DiscordMessage message)
        {
            if (message.Author.IsBot)
                return false;

            if (message.Content.StartsWith(Bot.Instance.Prefix))
                return false;

            return true;
        }
        
        public static DiscordEmbedBuilder BuildEmbed(this WizardConfig config)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor(config.AuthorName, iconUrl: config.AuthorIcon)
                .WithTitle(config.Headline)
                .WithDescription(config.Description)
                .WithColor(DiscordColor.Cyan);
            return builder;
        }

        public static DiscordEmbedBuilder BuildEmbed(this WizardConfig config, string footer)
        {
            return config.BuildEmbed()
                .WithFooter(footer);
        }

        public static async Task<MessageResult<string>> GetUserReply(this CommandContext context, TimeSpan? timeoutOverride = null)
        {
            var result = await context.Message.GetNextMessageAsync(timeoutOverride);

            if (result.TimedOut)
                throw new StopWizardException(string.Empty, $"Wizard timed out");

            return new MessageResult<string>()
            {
                Value = result.Result.Content,
                UserMessage = result.Result
            };
        }

        public static async Task<DiscordMessage> ReplyWithStatus(this CommandContext context, StatusCode statusCode, string message, string username = null)
        {
            string statusCodeText = string.Empty;
            DiscordColor statusColor = DiscordColor.Green;
            
            switch (statusCode)
            {
                case StatusCode.SUCCESS:
                    statusCodeText = "Success";
                    break;
                    case StatusCode.WARNING:
                        statusCodeText = "Warning";
                        statusColor = DiscordColor.Yellow;
                        break;
                    case StatusCode.ERROR:
                        statusCodeText = "Error";
                        statusColor = DiscordColor.Red;
                        break;
            }

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithTitle(statusCodeText)
                .WithDescription(message)
                .WithFooter(username)
                .WithColor(statusColor);

            return await context.Message.RespondAsync(embed: builder.Build());
        }

        public static async Task<MessageResult<T>> GetUserReply<T>(this CommandContext context, TimeSpan? timeoutOverride = null)
        {
            do
            {
                var result = await context.Message.GetNextMessageAsync(timeoutOverride);

                if (result.TimedOut)
                    throw new StopWizardException(string.Empty, "Stopping Wizard");

                try
                {
                    T value = (T) Convert.ChangeType(result.Result.Content, typeof(T));
                    return new MessageResult<T>()
                    {
                        Value = value,
                        UserMessage = result.Result
                    };
                }
                catch (Exception ex)
                {
                    await context.ReplyWithStatus(StatusCode.ERROR,$"Incorrect input. Expected a value of type '{typeof(T).Name}'", context.Message.Author.Username);
                }
            } while (true);
        }

        public static DiscordEmbedBuilder AddFields(this DiscordEmbedBuilder builder, IDictionary<string, string> collection)
        {
            foreach (var pair in collection)
            {
                if(string.IsNullOrEmpty(pair.Key)) 
                    continue;
                
                builder.AddField(pair.Key, pair.Value);
            }

            return builder;
        }
    }
}
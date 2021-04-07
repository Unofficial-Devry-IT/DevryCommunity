using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotApp.Common.Exceptions;
using Domain.Entities.ConfigTypes;
using Domain.Enums;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;

namespace BotApp.Interaction
{
    /// <summary>
    /// Extension methods that assist in various discord related interactions
    /// </summary>
    public static class BaseInteractions
    {
        /// <summary>
        /// Message which returns a value of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public struct MessageResult<T>
        {
            /// <summary>
            /// Value retrieved from user
            /// </summary>
            public T Value { get; set; }
            
            /// <summary>
            /// The message which contained the result
            /// </summary>
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
        
        /// <summary>
        /// Generate <see cref="DiscordEmbed"/> based off of <see cref="InteractionConfig"/>
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static DiscordEmbedBuilder BuildEmbed(this InteractionConfig config)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor(config.AuthorName, iconUrl: config.AuthorIcon)
                .WithTitle(config.Headline)
                .WithDescription(config.Description)
                .WithColor(DiscordColor.Cyan);
            return builder;
        }

        /// <summary>
        /// <seealso cref="BuildEmbed(Domain.Entities.ConfigTypes.InteractionConfig)"/> -- adds footer
        /// </summary>
        /// <param name="config"></param>
        /// <param name="footer">Text that should appear at the bottom of <see cref="DiscordEmbed"/></param>
        /// <returns></returns>
        public static DiscordEmbedBuilder BuildEmbed(this InteractionConfig config, string footer)
        {
            return config.BuildEmbed()
                .WithFooter(footer);
        }

        /// <summary>
        /// Interaction requires a <see cref="string"/> result
        /// </summary>
        /// <param name="context">Context which contains the basic info on how to reach out to user</param>
        /// <param name="timeoutOverride">Timeout (if applicable)</param>
        /// <returns></returns>
        /// <exception cref="StopInteractionException"></exception>
        public static async Task<MessageResult<string>> GetUserReply(this CommandContext context, TimeSpan? timeoutOverride = null)
        {
            var result = await context.Message.GetNextMessageAsync(timeoutOverride);

            // If the user did not respond in sufficient time -- exit the interaction
            if (result.TimedOut)
                throw new StopInteractionException(string.Empty, $"Interaction timed out");
            
            return new MessageResult<string>()
            {
                Value = result.Result.Content,
                UserMessage = result.Result
            };
        }
        
        /// <summary>
        /// Reply to the user with some sort of status code/message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="statusCode">Code which indicates what type of message this is</param>
        /// <param name="message">Text to appear in message</param>
        /// <param name="username">To whom this message is for (if applicable)</param>
        /// <returns></returns>
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

        /// <summary>
        /// Interaction which requires some type of user input. (string, int, double, whichever primitive type)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="timeoutOverride">Time in which the user must reply (if applicable)</param>
        /// <typeparam name="T">Type of data we're trying to get from the user</typeparam>
        /// <returns><see cref="MessageResult{T}"/></returns>
        /// <exception cref="StopInteractionException"></exception>
        public static async Task<MessageResult<T>> GetUserReply<T>(this CommandContext context, TimeSpan? timeoutOverride = null)
        {
            // Do-While ---> because we want to force the user to give us valid data of the type we're looking for
            do
            {
                var result = await context.Message.GetNextMessageAsync(timeoutOverride);
                
                // IF the user did not respond in sufficient time --> throw exception so we can exit the interaction
                if (result.TimedOut)
                    throw new StopInteractionException(string.Empty, "Stopping Wizard");

                try
                {
                    // Simple way of converting the user's response to the primitive type we want
                    // If an exception is thrown --> the user did not provide the type of data we wanted
                    
                    T value = (T) Convert.ChangeType(result.Result.Content, typeof(T));
                    return new MessageResult<T>()
                    {
                        Value = value,
                        UserMessage = result.Result
                    };
                }
                catch
                {
                    await context.ReplyWithStatus(StatusCode.ERROR,$"Incorrect input. Expected a value of type '{typeof(T).Name}'", context.Message.Author.Username);
                }
            } while (true);
        }

        private static DiscordEmbedBuilder Clone(this DiscordEmbedBuilder builder)
        {
            return new DiscordEmbedBuilder()
                .WithAuthor(name: builder.Author.Name, url: builder.Author.Url, iconUrl: builder.Author.IconUrl)
                .WithTitle(builder.Title)
                .WithDescription(builder.Description)
                .WithFooter(text: builder.Footer.Text, iconUrl: builder.Footer.IconUrl)
                .WithColor(builder.Color.Value);
        }

        /// <summary>
        /// Add collection of items as fields to the <see cref="DiscordEmbed"/>
        /// Due to Discord's limitations on fields (25 -- but we restrict to 24)
        /// Multiple embeds will be provided in the event the <paramref name="collection"/> contains >= 24 items
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="collection">Items to add</param>
        /// <returns><see cref="DiscordEmbedBuilder"/> array -- which represent pages</returns>
        public static DiscordEmbedBuilder[] AddFields(this DiscordEmbedBuilder builder, IDictionary<string, string> collection)
        {
            List<DiscordEmbedBuilder> embed = new List<DiscordEmbedBuilder>();

            int pages = (int) Math.Ceiling((double)collection.Count / 24);
            
            for (int i = 0; i < pages; i++)
            {
                DiscordEmbedBuilder clone = builder.Clone();
                
                var items = collection
                    .Skip(pages * 24)
                    .Take(24);

                // X of Total -- if total is > 1 page
                if (pages > 1)
                    clone.Title += $" Page [{i + 1} of {pages}]";

                foreach (var pair in items)
                {
                    // key's within a field cannot be blank -- causes an error
                    if (string.IsNullOrEmpty(pair.Key))
                        continue;

                    clone.AddField(pair.Key, pair.Value);
                }

                embed.Add(clone);
            }
            
            return embed.ToArray();
        }

        /// <summary>
        /// Instead of adding the <paramref name="collection"/> as fields -- they're added via string/text
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DiscordEmbedBuilder[] AddFieldsAsText(this DiscordEmbedBuilder builder,
            IDictionary<string, string> collection)
        {
            List<DiscordEmbedBuilder> embeds = new List<DiscordEmbedBuilder>();

            int totalLength = builder.Description.Length + CalculateTextCount(collection);
            
            // 2000 character limit for discord messages (that's their restriction - not ours)
            if(totalLength > 2000)
            {
                DiscordEmbedBuilder clone = builder.Clone();
                int pages = (int)Math.Ceiling((double)totalLength/2000);
                int currentPage = 1;
                int currentCount = clone.Description.Length;

                if (pages > 1)
                    clone.Title += $" [{currentPage} of {pages}]";
                
                embeds.Add(clone);
                
                while (collection.Count > 0)
                {
                    // Ensure the next set will not push us over the edge of 2000 characters
                    // 4 characters because [key]_value\n (brackets = 2, space = 1, newline = 1)
                    if(currentCount + collection.Select(x=>x.Key.Length + x.Value.Length + 4).Sum() > 2000)
                    {
                        clone = builder.Clone();
                        currentPage++;

                        if (pages > 1)
                            clone.Title += $" [{currentPage} of {pages}]";
                        
                        embeds.Add(clone);
                    }

                    var pair = collection.First();
                    
                    // this is required to prevent an infinite loop exception
                    collection.Remove(pair.Key);

                    clone.Description += $"[{pair.Key}] {pair.Value}\n";
                }
            }
            else
            {
                builder.Description += string.Join("\n", collection.Select(x => $"[{x.Key}] {x.Value}"));
                embeds.Add(builder);
            }
            
            return embeds.ToArray();
        }

        /// <summary>
        /// Assist with calculating total length of a collection (if appended as textual fields)
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        static int CalculateTextCount(IDictionary<string, string> collection)
        {
            // We're adding 4 because we'll be doing [key]_value\n (where _ = a space)
            return collection.Select(x => x.Key.Length + x.Value.Length + 4).Sum();
        }
    }
}
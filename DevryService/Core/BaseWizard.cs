using DevryService.Core.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core
{
    public abstract class BaseWizard
    {
        public string WizardName { get; protected set; }
        public string WizardIconUrl { get; protected set; }
        public string WizardTitle { get; protected set; }
        
        public ulong OriginalUserId { get; protected set; }
        public bool AcceptAnyUser { get; protected set; } = false;
        public bool MessageRequireMention { get; protected set; } = false;

        public DiscordChannel Channel { get; protected set; }
        public List<DiscordMessage> WizardMessages { get; protected set; } = new List<DiscordMessage>();

        public List<string> BlacklistedRoles = Worker.Configuration.GetValue<string[]>("blacklisted_roles").ToList();
        public bool NotBlacklisted(string name) => !BlacklistedRoles.Any(x => x.ToLower().Trim().Contains(name.ToLower().Trim()));
        const string CancelWizardMessage = "Type `stop` to cancel the wizard";
        protected CommandContext context;

        public BaseWizard(CommandContext context)
        {
            this.context = context;

            this.Channel = context.Channel;
            this.OriginalUserId = context.Member.Id;

            Type t = this.GetType();
            var attribute = t.GetCustomAttribute<WizardInfo>();

            if(attribute != null)
            {
                this.WizardName = attribute.Name;
                this.WizardIconUrl = attribute.IconUrl;
                this.WizardTitle = attribute.Title;
            }
        }

        /// <summary>
        /// Determine if message will be processed by this Wizard
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool GetUserResponsePredicate(DiscordMessage message)
        {
            if(message.ChannelId == Channel.Id)
            {
                if (!AcceptAnyUser && message.Author.Id != OriginalUserId)
                    return false;

                if (MessageRequireMention)
                {
                    if (message.MentionedUsers.Any(x => x.IsBot || x.Id == Bot.Discord.CurrentUser.Id))
                        return true;
                }
                else
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Send message to the channel (uses DiscordEmbed)
        /// </summary>
        /// <param name="text">text that appears in embed</param>
        /// <returns></returns>
        public async Task<DiscordMessage> SendMessage(string text)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();

            if (!string.IsNullOrEmpty(this.WizardName))
                builder = builder.WithAuthor(this.WizardName, icon_url: !string.IsNullOrEmpty(this.WizardIconUrl) ? this.WizardIconUrl : null);
            else if (!string.IsNullOrEmpty(this.WizardIconUrl))
                builder = builder.WithAuthor(icon_url: this.WizardIconUrl);

            builder.Title = this.WizardTitle;
            builder.Description = text;
            builder.Color = DiscordColor.Cyan;

            DiscordMessage message = await context.RespondAsync(embed: builder.Build());
            WizardMessages.Add(message);

            return message;
        }

        /// <summary>
        /// Modify the provided message to have the given contents
        /// </summary>
        /// <param name="message">The discord message to update</param>
        /// <param name="text">The text to either append or swap</param>
        /// <param name="append">Should the text be added to the end or overwrite</param>
        /// <returns></returns>
        public async Task<DiscordMessage> EditMessage(DiscordMessage message, string text, bool append=false)
        {
            if (append && message.Embeds.Count > 0)
                text = message.Embeds[0].Description + text;

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithFooter(CancelWizardMessage + (context.Member != null ? $"\n--{context.Member.Username}" : ""))
                .WithAuthor(WizardName, null, WizardIconUrl)
                .WithTitle(WizardTitle)
                .WithDescription(text)
                .WithColor(DiscordColor.Cyan);

            return await message.ModifyAsync(embed: builder.Build());
        }


        public async Task<(bool success, DiscordMessage message)> GetUserReplyGeneric()
        {
            MessageContext messageContext = await Bot.Interactivity.WaitForMessageAsync(GetUserResponsePredicate);

            while(true)
            {
                try
                {
                    if (messageContext.Message.Content.StartsWith(Bot.Prefix))
                    {
                        await messageContext.Message.RespondAsync($"You can't use bot commands within the {this.WizardName} Wizard");
                    }
                    else if (messageContext.Message.Content.ToLower().Trim() == "stop")
                    {
                        await messageContext.Message.RespondAsync($"{WizardName} Wizard stopped");
                        throw new StopWizardException(WizardName);
                    }
                    else
                    {
                        WizardMessages.Add(messageContext.Message);
                        return (true, messageContext.Message);
                    }
                }
                catch (UnauthorizedException ex)
                {
                    Console.WriteLine($"An error occurred while retrieving user reply...\n\t{ex.Message}");
                    await Cleanup();
                    return (false, null);
                }
            }
        }

        public async Task<(bool success, T option)> GetUserReplyOption<T>(List<T> collection, Func<List<T>, List<string>> getNames)
        {
            var names = getNames(collection);
            await SendMessage(CreateOptionList(names));

            while(true)
            {
                MessageContext messageContext = await Bot.Interactivity.WaitForMessageAsync(GetUserResponsePredicate);

                try
                {
                    if(messageContext.Message.Content.StartsWith(Bot.Prefix))
                    {
                        await messageContext.Message.RespondAsync($"You cannot use bot commands within the {WizardName} Wizard");
                        continue;
                    }
                    else if(messageContext.Message.Content.ToLower().Trim() == "stop")
                    {
                        await Cleanup();
                        await messageContext.Message.RespondAsync($"{WizardName} wizard has stopped");
                        throw new StopWizardException(WizardName);
                    }
                    else
                    {
                        WizardMessages.Add(messageContext.Message);

                        names = getNames(collection);
                        if(int.TryParse(messageContext.Message.Content, out int selection))
                        {
                            if(selection < 0 || selection >= names.Count)
                            {
                                await EditMessage(messageContext.Message, $"Expected a value between 0 and {collection.Count - 1}", true);
                                continue;
                            }

                            return (true, collection[selection]);
                        }
                        else
                        {
                            await EditMessage(messageContext.Message, $"Expected a value between 0 and {collection.Count - 1}", true);
                        }
                    }
                }
                catch(UnauthorizedException ex)
                {
                    Console.WriteLine($"An error occurred while retrieving user reply...\n\t{ex.Message}");
                }
            }
        }

        public async Task<(bool success, T[] options)> GetUserReplyOptions<T>(List<T> collection, Func<List<T>, List<string>> getNames)
        {
            List<T> selected = new List<T>();

            var names = getNames(collection);
            await SendMessage(CreateOptionList(names, true));

            while (true)
            {
                MessageContext context = await Bot.Interactivity.WaitForMessageAsync(GetUserResponsePredicate);

                try
                {
                    if(context.Message.Content.StartsWith(Bot.Prefix))
                    {
                        await context.Message.RespondAsync($"You cannot use bot commands within the {WizardName} Wizard");
                        continue;
                    }
                    else if(context.Message.Content.ToLower().Trim() == "stop")
                    {
                        await Cleanup();
                        await context.Message.RespondAsync($"{WizardName} wizard has stopped");
                        throw new StopWizardException(WizardName);
                    }
                    else
                    {
                        WizardMessages.Add(context.Message);

                        string[] split = context.Message.Content.Trim().Replace(",", " ").Split(" ");
                        foreach(string item in split)
                        {
                            if(int.TryParse(item, out int selection))
                            {
                                if(selection < 0 || selection >= collection.Count)
                                {
                                    await EditMessage(context.Message, $"Expected a value between 0 and {collection.Count - 1}", true);
                                    continue;
                                }

                                if (!selected.Contains(collection[selection]))
                                    selected.Add(collection[selection]);
                            }
                        }

                        break;
                    }
                }
                catch(UnauthorizedException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return (selected.Count > 0, selected.ToArray());
        }

        /// <summary>
        /// Allow a user to select an option from a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<(bool success, T option)> GetUserReplyOption<T>(List<T> collection)
        {
            var names = collection.Select(x => x.ToString()).ToList();
            await SendMessage(CreateOptionList(names));

            while(true)
            {
                MessageContext messageContext = await Bot.Interactivity.WaitForMessageAsync(GetUserResponsePredicate);
             
                try
                {
                    if(messageContext.Message.Content.StartsWith(Bot.Prefix))
                    {
                        await messageContext.Message.RespondAsync($"You cannot use bot commands within the {WizardName} Wizard");
                        continue;
                    }
                    else if(messageContext.Message.Content.ToLower().Trim() == "stop")
                    {
                        await Cleanup();
                        await messageContext.Message.RespondAsync($"{WizardName} wizard has stopped");
                        throw new StopWizardException(WizardName);
                    }
                    else
                    {
                        WizardMessages.Add(messageContext.Message);                        

                        if(int.TryParse(messageContext.Message.Content, out int selection))
                        {
                            if(selection < 0 || selection >= collection.Count)
                            {
                                await EditMessage(messageContext.Message, $"Expected a value between 0 and {collection.Count - 1}", true);
                                continue;
                            }

                            // Value provided was valid, so return a success message in addition to the option the user wanted
                            return (true, collection[selection]);
                        }
                        else
                        {
                            await EditMessage(messageContext.Message, $"Expected a value between 0 and {collection.Count - 1}", true);
                        }
                    }
                }
                catch (UnauthorizedException ex)
                {
                    Console.WriteLine($"An error occurred while retrieving user reply...\n\t{ex.Message}");                                    
                }
            }
        }

        /// <summary>
        /// Allow the user to select multiple items from a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<(bool success, T[] options)> GetUserReplyOptions<T>(List<T> collection)
        {
            List<T> options = new List<T>();
            var names = collection.Select(x => x.ToString()).ToList();
            await SendMessage(CreateOptionList(names, true));

            while(true)
            {
                MessageContext messageContext = await Bot.Interactivity.WaitForMessageAsync(GetUserResponsePredicate);

                try
                {
                    if (messageContext.Message.Content.StartsWith(Bot.Prefix))
                    {
                        await messageContext.Message.RespondAsync($"You cannot use bot commands within the {WizardName} Wizard");
                        continue;
                    }
                    else if (messageContext.Message.Content.ToLower().Trim() == "stop")
                    {
                        await Cleanup();
                        await messageContext.Message.RespondAsync($"{WizardName} wizard has stopped");
                        throw new StopWizardException(WizardName);
                    }
                    else
                    {
                        WizardMessages.Add(messageContext.Message);

                        string[] split = messageContext.Message.Content.Trim().Replace(",", " ").Split(" ");

                        foreach (string item in split)
                        {
                            if (int.TryParse(item, out int selection))
                            {
                                if (selection < 0 || selection >= collection.Count)
                                {
                                    await EditMessage(messageContext.Message, $"Expected a value between 0 and {collection.Count - 1}", true);
                                    continue;
                                }

                                // So long as the selected options does not contain the item we're currently trying to add
                                if (!options.Contains(collection[selection]))
                                    options.Add(collection[selection]);
                            }
                        }

                        // At this point we can assume we have something and can safely exit
                        break;
                    }
                }
                catch(UnauthorizedException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return (options.Count > 0, options.ToArray());
        }

        /// <summary>
        /// Create a list of options the user can pick from
        /// [i] - option
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected string CreateOptionList(List<string> collection, bool allowMultiple = false)
        {
            string multiple = allowMultiple ? "number(s)" : "number";
            string message = $"Please type in the corresponding {multiple} you're interested in\n";

            for(int i = 0; i < collection.Count; i++)
                message += $"[{i}] - {collection[i]}\n";

            return message;
        }

        /// <summary>
        /// Retrieve user reaction for a particular message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<DiscordEmoji> GetUserReactionTo(DiscordMessage message)
        {
            if(this.AcceptAnyUser)
            {
                ReactionContext reactionContext = await Bot.Interactivity.WaitForMessageReactionAsync(message);
                return reactionContext.Emoji;
            }
            else
            {
                ReactionContext reactionContext = await Bot.Interactivity.WaitForMessageReactionAsync(message, context.Member);
                return reactionContext.Emoji;
            }
        }

        public async Task Cleanup()
        {
            if (WizardMessages.Count <= 0)
                return;

            if (Channel.Type == DSharpPlus.ChannelType.Text)
                await Channel.DeleteMessagesAsync(WizardMessages);
        }

        public abstract Task StartWizard();
    }
}

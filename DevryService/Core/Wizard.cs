using DevryService.Commands;
using DevryService.Core.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core
{
    public abstract class Wizard
    {
        public string AuthorName { get; protected set; }
        public string AuthorIcon { get; protected set; }
        public string Title { get; protected set; }

        public ulong OriginalUserId { get; protected set; }
        public bool AcceptAnyUser { get; protected set; } = false;
        public bool MessageRequireMention { get; protected set; } = false;

        public DiscordChannel Channel { get; protected set; }
        public List<DiscordMessage> WizardMessages { get; protected set; } = new List<DiscordMessage>();

        public List<string> BlacklistedRoles = new List<string>
        {
            "@everyone",
            "Admin",
            "Moderator",
            "Pollmaster",
            "Professor",
            "Database",
            "Programmer",
            "Networking",
            "Hardware",
            "Motivator",
            "DeVry-SortingHat",
            "Devry-Service-Bot",
            "Devry-Challenge-Bot",
            "Devry Test Bot",
            "MathBot",
            "See-All-Channels",
            "Devry",
            "tutor"
        };

        public bool NotBlacklisted(string name) => !BlacklistedRoles.Any(x => x.ToLower().Trim().Contains(name.ToLower().Trim()));

        protected DiscordMember OriginalMember;
        const string CancelWizardMessage = "Type `stop` to cancel the wizard";

        protected DiscordMessage WizardMessage;

        public Wizard(ulong userId, DiscordChannel channel)
        {
            Type t = this.GetType();
            var attribute = t.GetCustomAttribute<WizardInfo>();

            this.AuthorName = attribute.Name;

            this.Channel = channel;
            this.OriginalUserId = userId;
            this.AuthorIcon = attribute.IconUrl;
            this.Title = attribute.Title;
        }

        /// <summary>
        /// Used to grab the user's response to this wizard
        /// </summary>
        /// <param name="message">Message to filter</param>
        protected virtual bool ResponsePredicate(DiscordMessage message)
        {
            if (message.ChannelId == Channel.Id)
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
        /// Generate a Wizard Reply with the text provided
        /// </summary>
        /// <param name="context"></param>
        /// <param name="text"></param>
        /// <param name="footer"></param>
        /// <returns></returns>
        public async Task<DiscordMessage> WizardReply(CommandContext context, string text, bool footer=true)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();

            if (footer)
                builder = builder.WithFooter($"{CancelWizardMessage}\n--{context.Message.Author.Username}");

            if (!string.IsNullOrEmpty(AuthorName))
                builder = builder.WithAuthor(AuthorName, iconUrl: !string.IsNullOrEmpty(AuthorIcon) ? AuthorIcon : null);
            else if (!string.IsNullOrEmpty(AuthorIcon))
                builder = builder.WithAuthor(iconUrl: AuthorIcon);

            builder.Title = this.Title;
            builder.Description = text;
            builder.Color = DiscordColor.Cyan;

            DiscordMessage message = await context.RespondAsync(embed: builder.Build());

            WizardMessages.Add(message);
            return message;
        }

        /// <summary>
        /// Update message provided
        /// </summary>
        /// <param name="message"></param>
        /// <param name="text"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public async Task<DiscordMessage> WizardReplyEdit(DiscordMessage message, string text, bool add=false)
        {
            if (add && message.Embeds.Count > 0)
                text = message.Embeds[0].Description + text;

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithFooter(CancelWizardMessage + (OriginalMember != null ? $"\n--{OriginalMember.Username}" : ""))
                .WithAuthor(AuthorName, null, AuthorIcon)
                .WithTitle(Title)
                .WithDescription(text)
                .WithColor(DiscordColor.Cyan);

            return await message.ModifyAsync(embed: builder.Build());
        }

        public async Task<DiscordMessage> GetUserReply()
        {
            var messageContext = await Bot.Interactivity.WaitForMessageAsync(ResponsePredicate);
            
            try
            {
                if (messageContext.Message.Content.StartsWith(Bot.Prefix))
                {
                    await messageContext.Message.RespondAsync($"You can't use bot commands within the {AuthorName} Wizard.");
                    throw new StopWizardException(AuthorName);
                }
                else if (messageContext.Message.Content.ToLower().Trim() == "stop")
                {
                    await messageContext.Message.RespondAsync($"{AuthorName} Wizard stopped");
                    throw new StopWizardException(AuthorName);
                }
                else
                {
                    WizardMessages.Add(messageContext.Message);
                    return messageContext.Message;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving user reply...\n\t{ex.Message}");
                await Cleanup();
                return null;
            }
        }

        public async Task<DiscordEmoji> GetUserReactionTo(DiscordMessage reactionTo)
        {
            ReactionContext context = await Bot.Interactivity.WaitForMessageReactionAsync(reactionTo, await reactionTo.Channel.Guild.GetMemberAsync(OriginalUserId));
            return context.Emoji;
        }

        /// <summary>
        /// Cleanup the wizard process
        /// </summary>
        /// <returns></returns>
        public async Task Cleanup()
        {
            if (WizardMessages.Count <= 0)
                return;

            if (Channel.Type == DSharpPlus.ChannelType.Text)
                await Channel.DeleteMessagesAsync(WizardMessages);
        }

        /// <summary>
        /// Starting point for all wizards
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task StartWizard(CommandContext context);
    }
}

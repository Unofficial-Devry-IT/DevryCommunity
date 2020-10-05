using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevryServiceBot.Util
{


    public abstract class Wizard
    {
        public string AuthorName { get; protected set; }
        public string AuthorIcon { get; protected set; }
        public string Name { get; private set; }

        /// <summary>
        /// User which started this wizard
        /// </summary>
        public ulong UserId { get; private set; }
        public bool IsActive { get; protected set; }

        /// <summary>
        /// Should general responses require the user who started the wizard?
        /// </summary>
        public bool AcceptAnyUser { get; protected set; } = false;

        /// <summary>
        /// If responding via message --- does the user HAVE to mention the bot?
        /// </summary>
        public bool MessageRequireMention { get; protected set; } = false;

        /// <summary>
        /// List or roles that cannot be utilized by the bot
        /// </summary>
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
            "Devry"
        };

        public bool NotBlacklisted(string name) => !BlacklistedRoles.Any(x => x.ToLower().Trim().Contains(name.ToLower().Trim()));

        /// <summary>
        /// Channel which this wizard was created on
        /// </summary>
        public DiscordChannel Channel { get; protected set; }

        /// <summary>
        /// Messages that are associated with this wizard's process
        /// </summary>
        public List<DiscordMessage> WizardMessages { get; } = new List<DiscordMessage>();
        

        public Wizard(ulong userId, DiscordChannel channel)
        {
            Type t = this.GetType();
            var attribute = t.GetCustomAttribute<WizardInfoAttribute>();

            this.Name = attribute.Name;
            this.AuthorIcon = attribute.IconUrl;
            this.AuthorName = attribute.Name;

            this.UserId = userId;
            this.IsActive = false;
            this.Channel = channel;
        }

        /// <summary>
        /// Used to grab the user's response to this wizard
        /// </summary>
        /// <param name="message">Message to filter</param>
        /// <returns>True if the user id AND channel id matches this wizard</returns>
        protected virtual bool ResponsePredicate(DiscordMessage message)
        {
            if(message.ChannelId == Channel.Id)
            {
                if (!AcceptAnyUser && message.Author.Id != UserId)
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

        public async Task<DiscordMessage> WizardReply(CommandContext context, string text, bool footer=true)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();

            if (footer)
                builder = builder.WithFooter($"Type `stop` to cancel the wizard\n--{context.Message.Author.Username}");

            if (!string.IsNullOrEmpty(AuthorName))
                builder = builder.WithAuthor(AuthorName, icon_url: !string.IsNullOrEmpty(AuthorIcon) ? AuthorIcon : null);
            else if (!string.IsNullOrEmpty(AuthorIcon))
                builder = builder.WithAuthor(icon_url: AuthorIcon);

            builder.Title = this.Name;
            builder.Description = text;

            builder.Color = DiscordColor.Cyan;

            var message = await context.RespondAsync(embed: builder.Build());

            WizardMessages.Add(message);
            return message;
        }

        public async Task<DiscordMessage> WizardReplyEdit(DiscordMessage message, string text, bool add=false)
        {
            if (add && message.Embeds.Count > 0)
                text = message.Embeds[0].Description + text;

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder().WithFooter("Type `stop` to cancel the wizard");
            builder.Title = this.Name;
            builder.Description = text;
            builder.Color = DiscordColor.Cyan;

            return await message.ModifyAsync(embed: builder.Build());
        }

        public async Task<DiscordMessage> AddValid(DiscordMessage message, string text)
        {
            string content = "";

            if (message.Embeds.Count > 0)
                content = message.Embeds[0].Description + "\n\n✅ " + text;

            return await WizardReplyEdit(message, content);
        }

        public async Task<DiscordMessage> AddError(DiscordMessage message, string error)
        {
            string content = "";

            if (message.Embeds.Count > 0)
                content = message.Embeds[0].Description + "\n\n:exclamation: " + error;

            return await WizardReplyEdit(message, content);
        }

        public async Task<DiscordEmoji> GetUserReactionTo(DiscordMessage reactionTo)
        {
            ReactionContext context = await Bot.Interactivity.WaitForMessageReactionAsync(reactionTo, await reactionTo.Channel.Guild.GetMemberAsync(UserId));
            return context.Emoji;
        }

        public async Task<DiscordMessage> GetUserReply(int timeout=-1)
        {
            MessageContext messageContext = await Bot.Interactivity.WaitForMessageAsync(ResponsePredicate);

            if (messageContext.Message.Content.StartsWith(Bot.Prefix))
            {
                await messageContext.Message.RespondAsync($"You can't use bot commands within the {Name} Wizard.");
                throw new StopWizardException(Name);
            }
            else if (messageContext.Message.Content.ToLower().Trim() == "stop")
            {
                await messageContext.Message.RespondAsync($"{Name} Wizard stopped");
                throw new StopWizardException(Name);
            }
            else
            {
                WizardMessages.Add(messageContext.Message);
                return messageContext.Message;
            }
        }

        /// <summary>
        /// Removes messages that were created for this wizard
        /// </summary>
        /// <param name="channel">Channel to clean up</param>
        /// <returns></returns>
        public async Task Cleanup()
        {
            if (WizardMessages.Count <= 0)
                return;

            if(Channel.Type == DSharpPlus.ChannelType.Text)
                await Channel.DeleteMessagesAsync(WizardMessages);
        }

        /// <summary>
        /// Kicks off the wizard
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task StartWizard(CommandContext context);

        /// <summary>
        /// Starts a long-running poll which will accumulate responses over time (via reactions)
        /// After <paramref name="pollSpan"/> has been met, the poll will finalize
        /// </summary>
        /// <param name="context">Context of the command being utilized</param>
        /// <param name="pollSpan">Duration of poll defined via <see cref="TimeSpan"/></param>
        /// <returns></returns>
        public virtual Task LongPollWithReactions(CommandContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts a long-running poll which will accumulate responses over time (via messages)
        /// After <paramref name="pollSpan"/> has been met, the poll will finalize
        /// </summary>
        /// <param name="context">Context of the command being utilized</param>
        /// /// <param name="pollSpan">Duration of poll defined via <see cref="TimeSpan"/></param>
        public virtual Task LongPollWithMessages(CommandContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Things that should happen once the poll has been completed
        /// </summary>
        /// <param name="context"></param>
        public virtual Task FinalizePoll(CommandContext context)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Does the text need to be paginated?
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected bool NeedsPagination(string text) => text.Length >= 2048;
    }
}

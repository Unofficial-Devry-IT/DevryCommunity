using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core
{
    #region Event Delegates to tap into
    public delegate Task UserReact(DiscordMessage message, DiscordUser user, DiscordEmoji emoji);
    public delegate Task UserReactRemoved(DiscordMessage message, DiscordUser user, DiscordEmoji emoji);
    public delegate Task UserReply(DiscordMessage message, DiscordUser user);
    #endregion

    public class Handler
    {
        static Handler instance;
        public static Handler Instance
        {
            get
            {
                if (instance == null)
                    instance = new Handler();
                return instance;
            }
        }

        #region Events to tap into
        public event UserReact OnUserReact;
        public event UserReactRemoved OnUserReactRemoved;
        public event UserReply OnUserReply;
        #endregion

        ~Handler()
        {
            Bot.Discord.MessageCreated -= DiscordMessageCreated;
            Bot.Discord.MessageReactionAdded -= DiscordMessageReaction;
            Bot.Discord.MessageReactionRemoved -= DiscordMessageReactionRemoved;
        }

        public Handler()
        {
            Bot.Discord.MessageCreated += DiscordMessageCreated;
            Bot.Discord.MessageReactionAdded += DiscordMessageReaction;
            Bot.Discord.MessageReactionRemoved += DiscordMessageReactionRemoved;
        }

        private async Task DiscordMessageReactionRemoved(MessageReactionRemoveEventArgs e)
        {
            if (OnUserReactRemoved == null) return;
            await OnUserReactRemoved(e.Message, e.User, e.Emoji);
        }

        private async Task DiscordMessageReaction(MessageReactionAddEventArgs e)
        {
            if (OnUserReact == null) return;
            await OnUserReact(e.Message, e.User, e.Emoji);
        }

        private async Task DiscordMessageCreated(MessageCreateEventArgs e)
        {
            if (OnUserReply == null) return;
            await OnUserReply(e.Message, e.Author);
        }
    }
}

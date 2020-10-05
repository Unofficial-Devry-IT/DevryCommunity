using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevryServiceBot.Util
{
    public delegate Task UserReact(DiscordMessage message, DiscordUser user, DiscordEmoji emoji);
    public delegate Task UserReactRemoved(DiscordMessage message, DiscordUser user, DiscordEmoji emoji);
    public delegate Task UserReply(DiscordMessage message, DiscordUser user);

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

        public event UserReact OnUserReact;
        public event UserReactRemoved OnUserReactRemoved;
        public event UserReply OnUserReply;

        /// <summary>
        /// Must ensure that when destroyed our events are properly removed/disposed
        /// </summary>
        ~Handler()
        {
            Bot.Discord.MessageCreated -= Discord_MessageCreated;
            Bot.Discord.MessageReactionAdded -= Discord_MessageReactionAdded;
            Bot.Discord.MessageReactionRemoved -= Discord_MessageReactionRemoved;
        }

        public Handler()
        {
            Bot.Discord.MessageCreated += Discord_MessageCreated;
            Bot.Discord.MessageReactionAdded += Discord_MessageReactionAdded;
            Bot.Discord.MessageReactionRemoved += Discord_MessageReactionRemoved;
        }

        private async Task Discord_MessageReactionRemoved(DSharpPlus.EventArgs.MessageReactionRemoveEventArgs e)
        {
            if (OnUserReactRemoved == null) return;
            await OnUserReactRemoved(e.Message, e.User, e.Emoji);
        }

        private async Task Discord_MessageReactionAdded(DSharpPlus.EventArgs.MessageReactionAddEventArgs e)
        {
            if (OnUserReact == null) return;
            await OnUserReact(e.Message, e.User, e.Emoji);
        }

        private async Task Discord_MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (OnUserReply == null) return;
            await OnUserReply(e.Message, e.Message.Author);
        }
    }
}

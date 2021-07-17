using System.Threading.Tasks;
using DSharpPlusNextGen.Entities;
using DSharpPlusNextGen.SlashCommands;
using DSharpPlusNextGen;

namespace DevryBot.Discord.Extensions
{
    public static class InteractionExtensions
    {

        /// <summary>
        /// Ensure the the interaction context contains a valid guild
        /// - Ran on slash commands
        /// </summary>
        /// <param name="context"></param>
        /// <returns>True if the guild is valid</returns>
        public static async Task<bool> ValidateGuild(this InteractionContext context)
        {
            if (context.Guild != null) 
                return true;

            DiscordInteractionResponseBuilder discordInteractionResponseBuilder = new()
            {
                Content = "Error: This is a guild command!",

                // Make all errors visible to just the user, makes the channel more clean for everyone else.
                IsEphemeral = true
            };

            // send the response
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                discordInteractionResponseBuilder);

            return false;
        }

        /// <summary>
        /// Update the interaction component to a 'thinking state'
        /// This provides visual feedback that the user is waiting on the server
        /// to do something
        /// </summary>
        /// <param name="context"></param>
        public static async Task ImThinking(this InteractionContext context)
        {
            // InteractionResponseType.DeferredChannelMessageWithSource let's the user know that we got the command,
            // and that the bot is "thinking" -- or really just taking a really long time
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new()
            {
                IsEphemeral = true
            });
        }

    }
}
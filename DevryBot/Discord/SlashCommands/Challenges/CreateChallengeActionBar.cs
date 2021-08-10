using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Discord.SlashCommands.Filters;
using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.SlashCommands;

namespace DevryBot.Discord.SlashCommands.Challenges
{
    public class CreateChallengeActionBar : SlashCommandModule
    {
        [SlashCommand("create-challenge-action-bar", "Actions for challenges")]
        [RequireModerator]
        public async Task Command(InteractionContext context)
        {
            if (!await context.ValidateGuild())
                return;

            DiscordButtonComponent cycleChallengeButton =
                new DiscordButtonComponent(ButtonStyle.Primary, InteractionConstants.NEW_CHALLENGE, "New Challenge", false, null);

            DiscordButtonComponent closeChallengeButton = new DiscordButtonComponent(ButtonStyle.Danger,
                InteractionConstants.END_CHALLENGE, "End Challenge", false, null);

            DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();
            responseBuilder.AddComponents(cycleChallengeButton, closeChallengeButton);
            responseBuilder.Content = "Challenge Action Bar";
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, responseBuilder);
        }
    }
}
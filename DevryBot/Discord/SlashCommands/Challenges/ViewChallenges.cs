using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Discord.SlashCommands.Filters;
using DevryBot.Options;
using DevryBot.Services;
using DevryDomain.Models;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.SlashCommands;
using Microsoft.Extensions.Options;

namespace DevryBot.Discord.SlashCommands.Challenges
{
    public class ViewChallenges : SlashCommandModule
    {
        public IGamificationService Service { get; set; }
        public IOptions<ChallengeOptions> Options { get; set; }

        private string Append(string original, Challenge entry)
        {
            return $"{original}\n{entry.Id}: {entry.Question}\n";
        }
        
        [SlashCommand("view-challenges", "View existing challenges")]
        [RequireModerator]
        public async Task Command(InteractionContext context)
        {
            if (!await context.ValidateGuild() || Options.Value.ViewChallengeChannelId != context.Channel.Id)
                return;

            // Not populating the add challenge channel with this content
            if (context.Channel.Id == Options.Value.AddChallengeChannelId)
                return;
            
            await context.ImThinking();

            var challenges = await Service.GetChallenges();

            foreach (var entry in challenges)
            {
                DiscordButtonComponent deleteButton = new DiscordButtonComponent(ButtonStyle.Danger,
                    $"{entry.Id}_{InteractionConstants.CHALLENGE_DELETE}", "Delete", false, null);
                
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                    .WithTitle("Challenge - " + entry.Id)
                    .WithColor(DiscordColor.Blurple)
                    .AddField("ID", entry.Id.ToString())
                    .AddField("Question", entry.Question);

                DiscordMessageBuilder builder = new DiscordMessageBuilder()
                    .WithEmbed(embed.Build())
                    .AddComponents(deleteButton);

                await context.Channel.SendMessageAsync(builder);
                await Task.Delay(500);
            }

            await context.EditResponseAsync(new DiscordWebhookBuilder());
        }
    }
}
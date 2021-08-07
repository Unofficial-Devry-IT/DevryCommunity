using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Options;
using DisCatSharp;
using DisCatSharp.Entities;
using DisCatSharp.SlashCommands;
using Microsoft.Extensions.Options;

namespace DevryBot.Discord.SlashCommands
{
    public class Invite : SlashCommandModule
    {
        public IOptions<DiscordOptions> DiscordOptions { get; set; }

        [SlashCommand("invite", "Get the invite link to the server!")]
        public async Task Command(InteractionContext context)
        {
            if (!await context.ValidateGuild())
                return;

            DiscordInteractionResponseBuilder responseBuilder = new();
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Invitation")
                .WithAuthor("Recruiting Hat")
                .WithDescription("Spread the word, our trusted scout! Spread the word " +
                                 "of our kingdom! Amass an army of knowledge seeking minions! Lay waste " +
                                 "to the legions of doubt and uncertainty!!")
                .AddField("Invite", "https://discord.io/unofficial-DevryIT")
                .WithFooter("Minions of knowledge! Assembblleeee!")
                .WithImageUrl(DiscordOptions.Value.InviteImage);

            responseBuilder.AddEmbed(embedBuilder.Build());

            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, responseBuilder);
        }
    }
}
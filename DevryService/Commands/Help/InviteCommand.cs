using DevryService.Core;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace DevryService.Commands.Help
{
    [Settings("inviteLinkConfig")]
    public class InviteCommand : BaseCommand
    {
        [Command("invite")]
        public override async Task ExecuteAsync(CommandContext context)
        {
            EmbedConfig config = Core.Util.ConfigHandler.InviteLinkConfig();

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor(config.AuthorName, iconUrl: config.AuthorIcon)
                .WithTitle(config.Title)
                .WithColor(DiscordColor.Cyan)
                .WithDescription(config.Contents)
                .WithFooter(config.Footer);

            foreach(var field in config.Fields)
            {
                string[] split = field.Split("|");
                if (split.Length != 2) continue;

                builder.AddField(split[0], split[1]);
            }

            await context.RespondAsync(embed: builder.Build());
        }
    }
}

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryInfrastructure.Persistence;
using DisCatSharp.Entities;
using DisCatSharp.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevryBot.Discord.SlashCommands.Gamification
{
    public class ViewStats : SlashCommandModule
    {
        public ILogger<ViewStats> Logger { get; set; }
        public IApplicationDbContext Context { get; set; }

        [SlashCommand("view-stats", "View your points!")]
        public async Task Command(InteractionContext context)
        {
            if (!await context.ValidateGuild())
                return;

            await context.ImThinking();

            // Get all the entries for the current user
            var entries = await Context.GamificationEntries
                .Where(x => x.UserId == context.Member.Id)
                .ToListAsync();

            DiscordWebhookBuilder responseBuilder = new();
            
            if (!entries.Any())
            {
                responseBuilder.AddEmbed(new DiscordEmbedBuilder()
                    .WithTitle($"{context.Member.DisplayName} Stats")
                    .WithDescription(
                        "You don't have any points right now. Please try participating in the community a bit more!")
                    .WithColor(DiscordColor.HotPink));

                await context.EditResponseAsync(responseBuilder);
                return;
            }

            double total = entries.Sum(x => x.Value);
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                .WithTitle($"{context.Member.DisplayName} Stats")
                .WithColor(DiscordColor.HotPink)
                .WithDescription($"You currently have a total of {total} points");

            foreach (var entry in entries)
            {
                var category = await Context.GamificationCategories.FindAsync(entry.GamificationCategoryId);
                if (string.IsNullOrEmpty(category.Name))
                    category.Name = "Uncategorized";
                embed.AddField(category.Name, entry.Value.ToString());
            }

            responseBuilder.AddEmbed(embed.Build());

            await context.EditResponseAsync(responseBuilder);
        }
    }
}
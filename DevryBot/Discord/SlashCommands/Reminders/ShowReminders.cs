using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Discord.SlashCommands.Filters;
using DevryInfrastructure.Persistence;
using DisCatSharp.Entities;
using DisCatSharp.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace DevryBot.Discord.SlashCommands.Reminders
{
    public class ShowReminders : SlashCommandModule
    {
        public IApplicationDbContext Context { get; set; }

        [SlashCommand("show-reminders", "Show all events for the current channel")]
        [RequireModerator]
        public async Task Command(InteractionContext context)
        {
            if (!await context.ValidateGuild())
                return;

            await context.ImThinking();

            DiscordWebhookBuilder responseBuilder = new();
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Scheduler")
                .WithFooter("Reminders for current channel");

            var reminders = await Context.Reminders
                .Where(x=>x.ChannelId == context.Channel.Id)
                .ToListAsync();

            if (reminders.Any())
            {
                embedBuilder.Description = string.Join("\n", reminders.Select(x => $"{x.Name} | {CronExpressionDescriptor.ExpressionDescriptor.GetDescription(x.Schedule)}"));
                embedBuilder.Color = DiscordColor.Green;
            }
            else
            {
                embedBuilder.Description = "No reminders exist for the current channel";
                embedBuilder.Color = DiscordColor.Yellow;
            }

            responseBuilder.AddEmbed(embedBuilder.Build());
            await context.EditResponseAsync(responseBuilder);
        }
    }
}
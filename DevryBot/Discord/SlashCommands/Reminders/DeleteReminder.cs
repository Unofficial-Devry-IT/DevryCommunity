using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Discord.SlashCommands.Filters;
using DevryBot.Services;
using DevryDomain.Models;
using DSharpPlusNextGen.Entities;
using DSharpPlusNextGen.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevryBot.Discord.SlashCommands.Reminders
{
    public class DeleteReminder : SlashCommandModule
    {
        [SlashCommand("delete-reminder", "Remove reminders from the current channel")]
        [RequireModerator]
        public static async Task Command(InteractionContext context)
        {
            if (!await context.ValidateGuild())
                return;

            await context.ImThinking();

            var reminderService = context.Services.GetService<ReminderBackgroundService>();
            
            DiscordWebhookBuilder responseBuilder = new();
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Scheduler")
                .WithFooter("Remove reminder(s)");

            var reminders = await Bot.Instance.Database.Reminders
                .Where(x => x.ChannelId == context.Channel.Id)
                .ToListAsync();

            if (!reminders.Any())
            {
                embedBuilder.Color = DiscordColor.Yellow;
                embedBuilder.Description = "No reminders exist for the current channel";

                responseBuilder.AddEmbed(embedBuilder.Build());
                await context.EditResponseAsync(responseBuilder);
                return;
            }

            List<DiscordSelectComponentOption> options = new();

            
            for(int i = 0; i < reminders.Count; i++)
            {
                var reminder = reminders[i];
                
                options.Add(new DiscordSelectComponentOption(
                    reminder.Name,
                    i.ToString(),
                    "",
                    false,
                    null));
            }
                
            string menuName = $"{context.User.Id}_removeReminder";
            DiscordSelectComponent menu = new DiscordSelectComponent(menuName,
                "Select the reminders you wish to remove",
                options.ToArray(), 1, options.Count);

            responseBuilder.AddComponents(menu);

            try
            {
                // update our thinking message to actual result
                var message = await context.EditResponseAsync(responseBuilder);

                var componentInteraction =
                    await Bot.Interactivity.WaitForSelectAsync(message, menuName, null);

                Bot.Instance
                    .Logger
                    .LogInformation(
                        $"The user interacted with the following: {string.Join(", ", componentInteraction.Result.Values)}");

                // Exit out of interaction if the user didn't select anything
                if (!componentInteraction.Result.Values.Any())
                {
                    embedBuilder.WithImageUrl(Bot.Instance.Configuration.WarningImage());
                    embedBuilder.Description = "No reminders were selected";
                    responseBuilder.AddEmbed(embedBuilder.Build());
                    await context.EditResponseAsync(responseBuilder);
                    return;
                }

                // The things they selected -- cache it for later
                List<Reminder> selectedReminders = new();
                foreach (var id in componentInteraction.Result.Values)
                    selectedReminders.Add(reminders[int.Parse(id)]);

                string text = string.Join("\n\t",
                    selectedReminders.OrderBy(x => x.Name)
                        .Select(x =>
                            $"{x.Name} | {CronExpressionDescriptor.ExpressionDescriptor.GetDescription(x.Schedule)}"));

                embedBuilder.Title = "In Queue to Remove";
                embedBuilder.Description = text;
                embedBuilder.ImageUrl = Bot.Instance.Configuration.QueueImage();

                responseBuilder = new();
                responseBuilder.AddEmbed(embedBuilder.Build());

                await context.EditResponseAsync(responseBuilder);

                foreach (var reminder in selectedReminders)
                {
                    await reminderService.Remove(reminder.Id.ToString());
                }

                await Bot.Instance.Database.SaveChangesAsync(default);
                await Task.Delay(TimeSpan.FromSeconds(5));
                
                // now tell the user we completed things
                embedBuilder.Title = "Reminders have been removed";
                embedBuilder.ImageUrl = Bot.Instance.Configuration.CompletedImage();

                responseBuilder = new();
                responseBuilder.AddEmbed(embedBuilder.Build());

                await context.EditResponseAsync(responseBuilder);
            }
            catch (Exception ex)
            {
                Bot.Instance.Logger.LogError(ex, "An error occurred while processing delete-reminder");
            }
        }
    }
}
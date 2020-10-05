using DevryServiceBot.Models;
using DevryServiceBot.Scheduling;
using DevryServiceBot.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryServiceBot.Wizards
{
    [WizardInfo(Name = "Procrastination Hat",
                IconUrl = "https://i1.pngguru.com/preview/218/44/945/free-time-icon-calendar-icon-text-line-pink-square-png-clipart-thumbnail.jpg",
                Description = "Did you make an oopsie? No longer need an event?",
                ReactionEmoji = ":date:",
                Group = "Event",
                CommandName = "EventCommands.DeleteEvent")]
    public class DeleteEventWizard : Wizard
    {
        public DeleteEventWizard(CommandContext context) : base(context.User.Id, context.Channel) {}

        public override async Task StartWizard(CommandContext context)
        {
            List<Reminder> reminders = new List<Reminder>();
            try
            {
                using (var database = new DevryDbContext())
                {
                    reminders = await database.Reminders
                                        .Where(x => x.ChannelId == context.Channel.Id && x.GuildId == context.Guild.Id)
                                        .ToListAsync();
                }
            }
            catch
            {
                await Cleanup();
                await context.RespondAsync(embed: new DiscordEmbedBuilder()
                                                    .WithTitle("Database Error")
                                                    .WithDescription("Could not retrieve events...")
                                                    .WithColor(DiscordColor.DarkRed)
                                                    .Build());
                return;
            }
            
            if(reminders.Count == 0)
            {
                await WizardReply(context, "No reminders are set for this channel...", false);
                return;
            }

            string baseMessage = "Please select the corresponding number(s) to delete a reminder\n";
            for (int i = 0; i < reminders.Count; i++)
                baseMessage += $"[{i + 1}] - {reminders[i].Title}\t{CronExpressionDescriptor.ExpressionDescriptor.GetDescription(reminders[i].CronString)}\n";

            DiscordMessage message = await WizardReply(context, baseMessage, true);

            DiscordMessage reply = await GetUserReply();

            string[] parameters = reply.Content.Replace(",", " ").Split(" ");

            List<string> removed = new List<string>();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    index -= 1;

                    if(index < 0 || index > reminders.Count)
                    {
                        await AddError(message, $"Invalid Range. Valid Range: 1 - {reminders.Count}");
                        continue;
                    }

                    SchedulerBackgroundService.Instance.RemoveTask(reminders[index].Id);

                    using (var database = new DevryDbContext())
                    {
                        database.Reminders.Remove(reminders[index]);
                        await database.SaveChangesAsync();
                    }
                    
                    removed.Add($"{reminders[index].Title} with Id {reminders[index].Id}");
                }
            }

            await Cleanup();
            if (removed.Count > 0)
                await WizardReply(context, $"Following events were removed: {string.Join("\n", removed)}");
            else
                await WizardReply(context, "No changes were made...");

        }
    }
}

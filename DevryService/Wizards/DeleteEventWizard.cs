using DevryService.Core;
using DevryService.Core.Schedule;
using DevryService.Core.Util;
using DevryService.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Wizards
{
    [WizardInfo(Name = "Procrastination Hat", Title = "Let's cancel that event!")]
    public class DeleteEventWizard : Wizard
    {
        public DeleteEventWizard(ulong userId, DiscordChannel channel) : base(userId, channel)
        {
        }

        public override async Task StartWizard(CommandContext context)
        {
            List<Reminder> reminders = new List<Reminder>();

            try
            {
                using (var database = new DevryDbContext())
                {
                    reminders = await database.Reminders
                        .Where(x => x.ChannelId == Channel.Id && x.GuildId == Channel.GuildId)
                        .ToListAsync();
                }
            }
            catch
            {
                await Cleanup();

                await context.RespondAsync(embed: new DiscordEmbedBuilder()
                                                        .WithTitle("Database Error")
                                                        .WithDescription("Could not retrieve events")
                                                        .WithColor(DiscordColor.IndianRed)
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
                baseMessage += $"[{i + 1}] - {reminders[i].Name}\t{CronExpressionDescriptor.ExpressionDescriptor.GetDescription(reminders[i].Schedule)}";

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
                        await WizardReplyEdit(message, $"Invalid Range. Valid Range: 1 - {reminders.Count}", true);
                        continue;
                    }

                    SchedulerBackgroundService.Instance.RemoveTask(reminders[index].Id);

                    using (var database = new DevryDbContext())
                    {
                        database.Reminders.Remove(reminders[index]);
                        await database.SaveChangesAsync();
                    }

                    removed.Add($"{reminders[index].Name} with Id {reminders[index].Id}");
                }
            }

            await Cleanup();

            if (removed.Count > 0)
                await WizardReply(context, $"Following events were removed: {string.Join("\n", removed)}", false);
            else
                await WizardReply(context, "No changes were made...", false);
        }
    }
}

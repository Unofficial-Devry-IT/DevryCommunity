using DevryService.Core;
using DevryService.Core.Schedule;
using DevryService.Core.Util;
using DevryService.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Wizards
{
    [WizardInfo(Name = "Planner Hat",
        Description = "Add/Remove Reminders?",
        IconUrl = "",
        Emoji = ":date:",
        Title = "Need a reminder?")]
    public class CreateEventWizard : Wizard
    {
        const string basicCron = "* | any value\n" +
                ", | value list separator\n" +
                "- | range of values\n" +
                "/ | step values\n\n";

        List<string> allowed = new List<string> { "Moderator", "Professor" };
        string[] daysOfWeek = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        string[] months = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEPT", "OCT", "NOV", "DEC" };
        const string baseMessage = "Event-Wizard: Follow the prompts below to create reminder\n\n";
        const string invalidInputMessage = "Invalid Input. Please checkout https://crontab.guru/ for assistance";

        public CreateEventWizard(ulong userId, DiscordChannel channel) : base(userId, channel)
        {
        }

        public override async Task StartWizard(CommandContext context)
        {
            if(!context.Member.Roles.Any(x=>allowed.Contains(x.Name)))
            {
                await WizardReply(context, "You do not have sufficient privileges to create a reminder!", false);
                return;
            }

            string headline = "",
                description = "",
                minutes = "",
                hours = "",
                days = "",
                month = "",
                daysOfWeek = "";

            WizardMessage = await WizardReply(context, baseMessage + "What should the headline be?", true);
            DiscordMessage reply = await GetUserReply();

            headline = reply.Content;
            description = await GetInput("What should the contents of this message be?");
            daysOfWeek = await GetDayOfWeek();
            month = await GetMonth();
            days = await GetRange("Day Interval", 1, 31);
            hours = await GetRange("Hour Interval", 0, 23);
            minutes = await GetRange("Minute Interval", 0, 59);

            string cronString = $"{minutes} {hours} {days} {month} {daysOfWeek}";

            Reminder reminder = new Reminder
            {
                ChannelId = context.Channel.Id,
                GuildId = context.Guild.Id,
                Contents = description,
                Name = headline,
                Schedule = cronString,
                NextRunTime = NCrontab.CrontabSchedule.Parse(cronString).GetNextOccurrence(DateTime.Now)
            };

            // TODO: Add Task to background service

            await Cleanup();

            try
            {
                SchedulerBackgroundService.Instance.AddTask(reminder);
                
                using (var database = new DevryDbContext())
                {
                    database.Reminders.Add(reminder);
                    await database.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();

                DiscordEmbed embed = builder.WithTitle("Database Error")
                    .WithDescription("Unable to save reminder to database...")
                    .WithColor(DiscordColor.DarkRed)
                    .Build();

                await context.RespondAsync(embed: embed);
                return;
            }

            string humanReadable = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(cronString);
            Worker.Instance.Logger.LogInformation($"Event Created:\n\tChannel -> {Channel.Name}\n\t{humanReadable}");
            await WizardReply(context, $"Event created!\n\n{humanReadable}", false);
        }

        async Task<string> GetDayOfWeek()
        {
            string display = baseMessage + "Day of Week Interval\n" + basicCron + "\n\n";

            for (int i = 0; i < daysOfWeek.Length; i++)
                display += $"[{i}] - {daysOfWeek[i]}\n";

            await WizardReplyEdit(WizardMessage, display, false);

            DiscordMessage reply = await GetUserReply();
            while(!ValidCron(reply.Content.Trim(), 0, 6))
            {
                await WizardReplyEdit(WizardMessage, ":exclamation: - " + invalidInputMessage, true);
                reply = await GetUserReply();
            }

            return reply.Content.Trim();
        }

        async Task<string> GetMonth()
        {
            string display = baseMessage + "Month Interval\n" + basicCron + "Valid Range: 1 - 12\n";

            for (int i = 0; i < months.Length; i++)
                display += $"[{i + 1}] - {months[i]}\n";

            await WizardReplyEdit(WizardMessage, display, false);

            DiscordMessage reply = await GetUserReply();

            while(!ValidCron(reply.Content.Trim(), 1, 12))
            {
                await WizardReplyEdit(WizardMessage, invalidInputMessage, true);
                reply = await GetUserReply();
            }

            return reply.Content.Trim();
        }

        async Task<string> GetRange(string customMessage, int min, int max)
        {
            string display = baseMessage + customMessage + "\n" + basicCron + $"Valid Range: {min} - {max}\n";
            await WizardReplyEdit(WizardMessage, display, false);

            DiscordMessage reply = await GetUserReply();

            while(!ValidCron(reply.Content.Trim(), min, max))
            {
                await WizardReplyEdit(WizardMessage, invalidInputMessage, true);
                reply = await GetUserReply();
            }

            return reply.Content.Trim();
        }

        async Task<string> GetInput(string message)
        {
            string display = baseMessage + message + "\n";
            await WizardReplyEdit(WizardMessage, display);

            DiscordMessage reply = await GetUserReply();

            return reply.Content.Trim();
        }

        bool ValidCron(string content, int? min = null, int? max = null)
        {
            foreach (var c in content)
                if (!(Char.IsDigit(c) || c == ',' || c == '-' || c == '/' || c == '*'))
                    return false;

            if (min.HasValue && max.HasValue && !content.Contains("*"))
            {
                string[] numbers = content.Replace(",", " ")
                                          .Replace("-", " ")
                                          .Replace("/", " ")
                                          .Split(" ");

                foreach (var selection in numbers)
                {
                    if (int.TryParse(selection, out int number))
                    {
                        if (number < min || number > max)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}

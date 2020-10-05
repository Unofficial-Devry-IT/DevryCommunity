using DevryServiceBot.Util;
using DSharpPlus.CommandsNext;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DevryServiceBot.Models;
using System.Net.Http;
using DevryServiceBot.Scheduling;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DevryServiceBot.Wizards
{
    [WizardInfo(Name = "Planner Hat", 
                Description = "Need a reminder?", 
                IconUrl = "https://i1.pngguru.com/preview/218/44/945/free-time-icon-calendar-icon-text-line-pink-square-png-clipart-thumbnail.jpg",
                ReactionEmoji = ":date:",
                Group = "Event",
                GroupDescription = "Add/Remove Reminders",
                CommandName="EventCommands.CreateEvent")]
    public class CreateEventWizard : Wizard
    {
        List<string> allowed = new List<string> { "Moderator", "Professor" };
        string[] daysOfWeek = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        string[] months = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEPT", "OCT", "NOV", "DEC" };
        const string baseMessage = "Event-Wizard: Follow the prompts below to create reminder\n\n";
        const string invalidInputMessage = "Invalid Input. Please checkout https://crontab.guru/ for assistance";

        public CreateEventWizard(CommandContext context) : base(context.User.Id, context.Channel) {}

        const string basicCron = "* | any value\n" +
                ", | value list separator\n" +
                "- | range of values\n" +
                "/ | step values\n\n";

        bool ValidCron(string content, int? min = null, int? max = null)
        {
            foreach (var c in content)
                if (!(Char.IsDigit(c) || c == ',' || c == '-' || c == '/' || c == '*'))
                    return false;

            if(min.HasValue && max.HasValue && !content.Contains("*"))
            {
                string[] numbers = content.Replace(",", " ")
                                          .Replace("-", " ")
                                          .Replace("/", " ")
                                          .Split(" ");

                foreach(var selection in numbers)
                {
                    if(int.TryParse(selection, out int number))
                    {
                        if (number < min || number > max)
                            return false;
                    }
                }
            }

            return true;
        }

        private async Task<string> GetDayOfWeek(DiscordMessage botMessage)
        {
            string display = baseMessage + "Day Of Week Interval\n" + basicCron + "\n\n";

            for (int i = 0; i < daysOfWeek.Length; i++)
                display += $"[{i}] - {daysOfWeek[i]}\n";

            await WizardReplyEdit(botMessage, display, false);

            DiscordMessage reply = await GetUserReply();

            while (!ValidCron(reply.Content.Trim(),0,6))
            {
                await AddError(botMessage, invalidInputMessage);
                reply = await GetUserReply();
            }

            return reply.Content.Trim();
        }

        private async Task<string> GetMonth(DiscordMessage message)
        {
            string display = baseMessage + "Month Interval\n" + basicCron + "Valid range: 1 - 12\n";

            for (int i = 0; i < months.Length; i++)
                display += $"[{i + 1}] - {months[i]}\n";

            await WizardReplyEdit(message, display, false);

            DiscordMessage reply = await GetUserReply();

            while(!ValidCron(reply.Content.Trim(), 1, 12))
            {
                await AddError(message, invalidInputMessage);
                reply = await GetUserReply();
            }

            return reply.Content.Trim();
        }

        private async Task<string> GetRange(DiscordMessage message, string customMessage, int min, int max)
        {
            string display = baseMessage + customMessage + "\n" + basicCron + $"Valid Range: {min} - {max}\n";
            await WizardReplyEdit(message, display, false);

            DiscordMessage reply = await GetUserReply();
            while (!ValidCron(reply.Content.Trim(), min, max))
            {
                await AddError(message, invalidInputMessage);
                reply = await GetUserReply();
            }

            return reply.Content.Trim();
        }

        private async Task<string> GetInput(DiscordMessage botMessage, string message)
        {
            string display = baseMessage + message + "\n";
            await WizardReplyEdit(botMessage, display);

            DiscordMessage reply = await GetUserReply();

            return reply.Content;
        }

        public override async Task StartWizard(CommandContext context)
        {
            if(!context.Member.Roles.Any(x => allowed.Contains(x.Name)))
            {
                await WizardReply(context, "You do not have sufficient privileges to create a reminder!", false);
                return;
            }

            string headline = "",
                   description = "",
                   minutes = "",
                   hours = "",
                   days = "",
                   months = "",
                   daysOfWeek = "";
            
            DiscordMessage message = await WizardReply(context, baseMessage + "What should the headline be?", true);
            DiscordMessage reply = await GetUserReply();

            headline = reply.Content;
            description = await GetInput(message, "What should the contents of this message be?");
            daysOfWeek = await GetDayOfWeek(message);
            months = await GetMonth(message);
            days = await GetRange(message, "Day Interval", 1, 31);
            hours = await GetRange(message, "Hour Interval", 0, 23);
            minutes = await GetRange(message, "Minute Interval", 0, 59);

            string cronstring = $"{minutes} {hours} {days} {months} {daysOfWeek}";

            Reminder reminder = new Reminder
            {
                ChannelId = context.Channel.Id,
                GuildId = context.Guild.Id,
                Contents = description,
                Title = headline,
                CronString = cronstring,
                NextRunTime = NCrontab.CrontabSchedule.Parse(cronstring).GetNextOccurrence(DateTime.Now)
            };

            // We need to add this task to our background service (otherwise our reminder was created for nothing)
            SchedulerBackgroundService.Instance.AddTask(reminder);

            await Cleanup();

            // Add our reminder to the database for persistence
            try
            {
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

            string humanReadable = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(cronstring);
            await WizardReply(context, $"Event created!\n{humanReadable}", false);
            
            Console.WriteLine($"Create Event: {reminder}");
        }
    }
}

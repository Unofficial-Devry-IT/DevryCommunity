using DevryService.Core;
using DevryService.Core.Schedule;
using DevryService.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Wizards.Admin
{
    public class CreateEventWizardConfig : WizardConfig
    {
        public List<string> AllowedToUse = new List<string>() { "Moderator" };
    }

    public class CreateEventWizard : WizardBase<CreateEventWizardConfig>
    {
        const string cronUsageMenu = "* | any value\n" +
            ", | value list separator\n" +
            "- | range of values\n" +
            "/ | step values\n\n";

        string[] daysOfWeek = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        string[] months = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEPT", "OCT", "NOV", "DEC" };

        const string baseMessage = "Event-Wizard: Follow the prompts below to create an event/reminder\n\n";
        const string invalidInputMessage = "Invalid Input. Please checkout https://crontab.guru/ for assistance";        

        public CreateEventWizard(CommandContext commandContext) : base(commandContext)
        {
        }

        protected override async Task<bool> ResponsePredicate(DiscordMessage message)
        {
            var member = await message.Channel.Guild.GetMemberAsync(message.Author.Id);

            var memberRoles = member.Roles.Select(x => x.Name.ToLower()).ToList();
            
            // user must have at least one of the roles defined in the options
            if (memberRoles.Any(x => _options.AllowedToUse.Any(y=>y.Equals(x,StringComparison.OrdinalIgnoreCase))))
                return true;

            return false;
        }

        public override CreateEventWizardConfig DefaultSettings()
        {
            CreateEventWizardConfig config = new CreateEventWizardConfig();

            config.Name = "Planner Hat";
            config.Description = "Add/Remove Events/Reminders";
            config.Icon = "";
            config.ReactionEmoji = ":date:";
            config.Title = "Need a reminder/event?";

            config.AllowedToUse = new List<string>()
            {
                "Moderator",
                "Professor",
                "Tutor"
            };

            config.MessageRequireMention = false;
            config.AcceptAnyUser = false;

            return config;
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

        async Task<string> GetDayOfWeekAsync()
        {
            string display = baseMessage + "Day of Week Interval\n" + cronUsageMenu + "\n\n";

            for (int i = 0; i < daysOfWeek.Length; i++)
                display += $"[{i}] - {daysOfWeek[i]}\n";

            string value = string.Empty;
            await ReplyEditWithReply(_recentMessage, display, false, true, (context) => value = context.Result.Content);

            while(!ValidCron(value.Trim(), 0, 6))
            {
                await ReplyEditWithReply(_recentMessage, ":exclamation: - " + invalidInputMessage, true, true, (context) => value = context.Result.Content);
            }

            return value;
        }

        async Task<string> GetMonth()
        {
            string display = baseMessage + "Month Interval\n" + cronUsageMenu + "Valid Range: 1-12\n";

            for (int i = 0; i < months.Length; i++)
                display += $"[{i + 1}] - {months[i]}\n";

            string value = string.Empty;
            await ReplyEditWithReply(_recentMessage, display, false, true, (context) => value = context.Result.Content);

            while (!ValidCron(value.Trim(), 1, 12))
            {
                await ReplyEditWithReply(_recentMessage, ":exclamation: - " + invalidInputMessage, true, true, (context) => value = context.Result.Content);
            }

            return value;
        }

        async Task<string> GetRange(string customMessage, int min, int max)
        {
            string display = baseMessage + customMessage + "\n" + cronUsageMenu + $"Valid Range: {min} - {max}\n";
            string value = string.Empty;

            await ReplyEditWithReply(_recentMessage, display, false, true, (context) => value = context.Result.Content);
            while(!ValidCron(value.Trim(), min, max))
            {
                await ReplyEditWithReply(_recentMessage, ":exclamation: - " + invalidInputMessage, true, true, (context) => value = context.Result.Content);
            }

            return value;
        }

        protected override async Task ExecuteAsync(CommandContext context)
        {
            if (!await ResponsePredicate(context.Message))
            {
                logger.LogWarning($"{context.Message.Author.Username} - attempted to use a command they don't have access to");
                return;
            }

            string headline = string.Empty,
                description = string.Empty,
                minutes = string.Empty,
                hours = string.Empty,
                days = string.Empty,
                month = string.Empty,
                daysOfWeek = string.Empty;

            _recentMessage = await WithReply(context, "What should the headline be?",
                (context) => headline = context.Result.Content,
                true);

            _recentMessage = await ReplyEditWithReply(_recentMessage, "What should the contents of this message be?", false, true,
                (context) => description = context.Result.Content);

            daysOfWeek = await GetDayOfWeekAsync();
            month = await GetMonth();
            days = await GetRange("Day Interval", 1, 31);
            hours = await GetRange("Hour Interval", 0, 23);
            minutes = await GetRange("Minute Interval", 0, 59);

            // CREATE REMINDER
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

            int statusCode = await Bot.Instance.DiscordService.CreateReminder(reminder);

            if(statusCode != DiscordService.OK)
            {
                await SimpleReply(context, "Sorry, an error occurred while processing your request. --Try again, then contact a moderator if the issue persists", false, false);
                throw new InvalidOperationException($"Unable to create reminder. Status Code: {statusCode}\n\tCron String: {cronString}");
            }
         
            SchedulerBackgroundService.Instance.AddTask(reminder);

            string humanReadable = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(cronString);
            Worker.Instance.Logger.LogInformation($"Event Created:\n\tChannel->{_channel.Name}\n\t{humanReadable}");
            await SimpleReply(context, $"Event Created!\n\n{humanReadable}", false, false);
        }
    }
}

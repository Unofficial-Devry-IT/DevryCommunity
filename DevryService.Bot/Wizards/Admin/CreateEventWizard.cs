using DevryService.Bot.Exceptions;
using DevryService.Database;
using DevryService.Database.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryService.Bot.Wizards.Admin
{
    public class CreateEventWizard : WizardBase
    {
        struct ExtensionConfig
        {
            public List<string> AllowedToUse;
        }

        const string CRON_USAGE_MENU = "* | any value\n" +
           ", | value list separator\n" +
           "- | range of values\n" +
           "/ | step values\n\n";

        string[] daysOfWeek = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        string[] months = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEPT", "OCT", "NOV", "DEC" };

        const string BASE_MESSAGE = "Event-Wizard: Follow the prompts below to create an event/reminder\n\n";
        const string INVALID_INPUT_MESSAGE = "Invalid Input. Please checkout https://crontab.guru/ for assistance";

        public CreateEventWizard(CommandContext context) : base(context)
        {
        }

        protected override async Task ExecuteAsync()
        {
            var originalResult = await WithReply<string>(BasicEmbed().WithDescription("What should the headline be?"));

            if(originalResult.message == null)
            {
                await SimpleReply(StoppedEmbed(), false);
                throw new StopWizardException(AuthorName);
            }

            string headline = originalResult.value,
                description = string.Empty,
                minutes = string.Empty,
                hours = string.Empty,
                days = string.Empty,
                month = string.Empty,
                daysOfWeek = string.Empty;

            description = await ReplyWithEdit<string>(originalResult.message, BasicEmbed().WithDescription("What should the contents of this message be?").Build());
            daysOfWeek = await GetDayOfWeekAsync(originalResult.message);
            month = await GetMonth(originalResult.message);
            days = await GetRange(originalResult.message, "Day Interval", 1, 31);
            hours = await GetRange(originalResult.message, "Hour Interval", 0, 23);
            minutes = await GetRange(originalResult.message, "Minute Interval", 0, 59);

            string cronString = $"{minutes} {hours} {days} {month} {daysOfWeek}";

            Reminder reminder = new Reminder
            {
                ChannelId = Context.Channel.Id,
                GuildId = Context.Guild.Id,
                Contents = description,
                Name = headline,
                Schedule = cronString,
                NextRunTime = NCrontab.CrontabSchedule.Parse(cronString).GetNextOccurrence(DateTime.Now)
            };

            await Database.Reminders.AddAsync(reminder);
            await Database.SaveChangesAsync();

            // TODO: Background Service
        }

        protected override string GetDefaultAuthorIcon() => null;
        protected override string GetDefaultAuthorName() => "Planner Hat";
        protected override string GetDefaultDescription() => "AddRemove Events/Reminders";
        protected override string GetDefaultHeadline() => "Need a reminder/event?";
        protected override TimeSpan? GetDefaultTimeoutOverride() => null;
        protected override string GetDefaultExtensionData() => Newtonsoft.Json.JsonConvert.SerializeObject(new ExtensionConfig
        {
            AllowedToUse = new List<string>()
            {
                "Moderator",
                "Professor",
                "Tutor"
            }
        });

        /// <summary>
        /// Determines if the provided content is valid cron string
        /// </summary>
        /// <param name="content"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>True: Valid cron | Otherwise false</returns>
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

        async Task<string> GetDayOfWeekAsync(DiscordMessage primaryMessage)
        {
            string display = BASE_MESSAGE + "Day of Week Interval\n" + CRON_USAGE_MENU + "\n\n";

            var embed = BasicEmbed().WithDescription(display);

            for (int i = 0; i < daysOfWeek.Length; i++)
                embed.AddField(i.ToString(), daysOfWeek[i], true);

            string value = await ReplyWithEdit<string>(primaryMessage, embed.Build());

            while(!ValidCron(value.Trim(), 0, 6))
            {
                embed.Description = $"\n:exclamation: Invalid Input. Exepected a value between 0 - {daysOfWeek.Length - 1}";
                value = await ReplyWithEdit<string>(primaryMessage, embed.Build());
            }

            return value;
        }

        async Task<string> GetMonth(DiscordMessage primaryMessage)
        {
            string display = BASE_MESSAGE + "Month Interval\n" + CRON_USAGE_MENU + "Valid Range: 1-12\n";

            var embed = BasicEmbed().WithAuthor(display);

            for (int i = 0; i < months.Length; i++)
                embed.AddField((i + 1).ToString(), months[i], true);

            string value = await ReplyWithEdit<string>(primaryMessage, embed.Build());

            while(!ValidCron(value.Trim(), 1, 12))
            {
                embed.Description = $"\n:exclamation: Invalid Input. Expected a value between 1 - {months.Length}";
                value = await ReplyWithEdit<string>(primaryMessage, embed.Build());
            }

            return value;
        }

        async Task<string> GetRange(DiscordMessage primaryMessage, string customMessage, int min, int max)
        {
            string display = BASE_MESSAGE + customMessage + "\n" + CRON_USAGE_MENU + $"Valid Range: {min} - {max}\n";
            var embed = BasicEmbed().WithDescription(display);

            string value = await ReplyWithEdit<string>(primaryMessage, embed.Build());

            while(!ValidCron(value.Trim(), min, max))
            {
                display += $"\n:exclamation: - {INVALID_INPUT_MESSAGE}";
                value = await ReplyWithEdit<string>(primaryMessage, embed.Build());
            }

            return value;
        }
    }
}

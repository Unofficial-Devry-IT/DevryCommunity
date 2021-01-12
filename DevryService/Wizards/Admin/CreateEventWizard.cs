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
        const string AUTHOR_NAME = "Planner Hat";
        const string DESCRIPTION = "Add/Remove Events/Reminders";
        const string REACTION_EMOJI = ":date:";
        const string AUTHOR_ICON = "";

        readonly List<string> ALLOWED_TO_USE = new List<string>()
        {
            "Moderator",
            "Professor",
            "Tutor"
        };

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

        protected override bool ResponsePredicate(DiscordMessage message)
        {
            var member = message.Channel.Guild.GetMemberAsync(message.Author.Id).GetAwaiter().GetResult();

            var memberRoles = member.Roles.Select(x => x.Name.ToLower()).ToList();
            
            // user must have at least one of the roles defined in the options
            if (memberRoles.Any(x => _options.AllowedToUse.Any(y=>y.Equals(x,StringComparison.OrdinalIgnoreCase))))
                return true;

            return false;
        }

        public override CommandConfig DefaultCommandConfig()
        {
            return new CommandConfig
            {
                AuthorName = AUTHOR_NAME,
                Description = DESCRIPTION,
                IgnoreHelpWizard = false,
                ReactionEmoji = REACTION_EMOJI,
                RestrictedRoles = ALLOWED_TO_USE,
                AuthorIcon = AUTHOR_ICON
            };
        }

        public override CreateEventWizardConfig DefaultSettings()
        {
            CreateEventWizardConfig config = new CreateEventWizardConfig();

            config.AuthorName = AUTHOR_NAME;
            config.Description = DESCRIPTION;
            config.AuthorIcon = AUTHOR_ICON;
            config.ReactionEmoji = REACTION_EMOJI;
            config.Headline = "Need a reminder/event?";

            config.AllowedToUse = ALLOWED_TO_USE;

            config.MessageRequireMention = false;
            config.AcceptAnyUser = false;

            config.UsesCommand = new WizardToCommandLink
            {
                DiscordCommand = "create-event",
                CommandConfig = DefaultCommandConfig()
            };

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
            var embed = EmbedBuilder()
                .WithDescription(display);

            for (int i = 0; i < daysOfWeek.Length; i++)
                embed.AddField(i.ToString(), daysOfWeek[i], true);

            string value = await ReplyEditWithReply<string>(_recentMessage, embed.Build());

            while(!ValidCron(value.Trim(), 0, 6))
            {
                embed.Description += $"\n:exclamation: Invalid Input. Expected a value between 0 - {daysOfWeek.Length - 1}";
                value = await ReplyEditWithReply<string>(_recentMessage, embed.Build());
            }

            return value;
        }

        async Task<string> GetMonth()
        {
            string display = baseMessage + "Month Interval\n" + cronUsageMenu + "Valid Range: 1-12\n";
            var embed = EmbedBuilder()
                .WithDescription(display);

            for (int i = 0; i < months.Length; i++)
                embed.AddField((i + 1).ToString(), months[i], true);

            string value = await ReplyEditWithReply<string>(_recentMessage, embed.Build());

            while (!ValidCron(value.Trim(), 1, 12))
            {
                embed.Description += $"\n:exclamation: Invalid Input. Expected a value between 1 - {months.Length}";
                value = await ReplyEditWithReply<string>(_recentMessage, embed.Build());
            }

            return value;
        }

        async Task<string> GetRange(string customMessage, int min, int max)
        {
            string display = baseMessage + customMessage + "\n" + cronUsageMenu + $"Valid Range: {min} - {max}\n";
            var embed = EmbedBuilder().WithDescription(display);

            string value = await ReplyEditWithReply<string>(_recentMessage, embed.Build());
            
            while(!ValidCron(value.Trim(), min, max))
            {
                display += $"\n:exclamation: - {invalidInputMessage}";
                value = await ReplyEditWithReply<string>(_recentMessage, embed.Build());
            }

            return value;
        }

        protected override async Task ExecuteAsync()
        {
            if (!ResponsePredicate(_context.Message))
            {
                logger.LogWarning($"{_context.Message.Author.Username} - attempted to use a command they don't have access to");
                return;
            }

            string headline = string.Empty,
                description = string.Empty,
                minutes = string.Empty,
                hours = string.Empty,
                days = string.Empty,
                month = string.Empty,
                daysOfWeek = string.Empty;

            _recentMessage = await WithReply("What should the headline be?",
                (context) => headline = context.Result.Content,
                true);
            
            description = await ReplyEditWithReply<string>(_recentMessage,
                EmbedBuilder()
                .WithDescription("What should the contents of this message be?").Build());

            daysOfWeek = await GetDayOfWeekAsync();
            month = await GetMonth();
            days = await GetRange("Day Interval", 1, 31);
            hours = await GetRange("Hour Interval", 0, 23);
            minutes = await GetRange("Minute Interval", 0, 59);

            // CREATE REMINDER
            string cronString = $"{minutes} {hours} {days} {month} {daysOfWeek}";
            
            Reminder reminder = new Reminder
            {
                ChannelId = _context.Channel.Id,
                GuildId = _context.Guild.Id,
                Contents = description,
                Name = headline,
                Schedule = cronString,
                NextRunTime = NCrontab.CrontabSchedule.Parse(cronString).GetNextOccurrence(DateTime.Now)
            };

            int statusCode = await Worker.Instance.DiscordService.CreateReminder(reminder);

            if(statusCode != DiscordService.OK)
            {
                await SimpleReply("Sorry, an error occurred while processing your request. --Try again, then contact a moderator if the issue persists", false, false);
                throw new InvalidOperationException($"Unable to create reminder. Status Code: {statusCode}\n\tCron String: {cronString}");
            }
         
            SchedulerBackgroundService.Instance.AddTask(reminder);

            string humanReadable = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(cronString);
            Worker.Instance.Logger.LogInformation($"Event Created:\n\tChannel->{_channel.Name}\n\t{humanReadable}");
            await SimpleReply($"Event Created!\n\n{humanReadable}", false, false);
        }
    }
}

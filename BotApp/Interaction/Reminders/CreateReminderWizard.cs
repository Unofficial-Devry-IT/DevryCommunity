using System.Threading.Tasks;
using Application.Reminders.Commands;
using BotApp.Extensions;
using DSharpPlus.CommandsNext;
using DevryServices.Common.Extensions;
using DevryServices.Common.Helpers;
using Domain.Entities.Configs.ConfigTypes;
using Domain.Enums;

namespace BotApp.Interaction.Reminders
{
    public class CreateReminderWizard : WizardBase
    {
        private const string CRON_GUIDE    = "* | any value\n" +
                                             ", | value list separator\n" +
                                             "- | range of values\n" +
                                             "/ | step values\n\n";
        
        public CreateReminderWizard(CommandContext context) : base(context)
        {
        }

        public override string WizardName() => "Event Wizard";

        protected override async Task ExecuteAsync(CommandContext context)
        {
            CreateReminderCommand command = new CreateReminderCommand();
            command.GuildId = context.Guild.Id;
            command.ChannelId = context.Channel.Id;

            string headline = await RetrieveData<string>("What should the headline be?", true);
            string description = await RetrieveData<string>("What should the contents of this message be?", true);
            
            string daysOfWeek = await GetDayOfWeek(),
                month = await GetMonth(),
                days = await GetRange("Day Interval", 1, 31),
                hours = await GetRange("Hour Interval", 0, 23),
                minutes = await GetRange("Minute Interval", 0, 59);
            
            
            command.Schedule = $"{minutes} {hours} {days} {month} {daysOfWeek}";
            command.Name = headline.ToDiscordMessage(context.Guild, context.Member);
            command.Contents = description.ToDiscordMessage(context.Guild, context.Member);
            string cronDescription = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(command.Schedule);
            
            // Send our command up the chain
            await Mediator.Send(command);

            await context.ReplyWithStatus(StatusCode.SUCCESS, $"Processing reminder: {cronDescription}");
        }

        private const string BASE_MESSAGE = "Following the prompts below to create an event/reminder\n\n";
        
        private async Task<string> GetDayOfWeek()
        {
            string message = BASE_MESSAGE + "Day of week interval\n" + CRON_GUIDE + "\n\n";

            var embed = CurrentConfig.BuildEmbed(InvokingUser)
                .WithDescription(message)
                .AddFields(EnumHelper.ToMenuDictionary(typeof(DayOfWeek)));

            
            string reply = await RetrieveData<string>(embed, true);

            while (!reply.IsValidCron(0, 6))
            {
                TrackedMessages.Add(await Context.ReplyWithStatus(StatusCode.WARNING, "Invalid cron string.", InvokingUser));
                reply = await RetrieveData<string>(embed, true);
            }

            return reply;
        }

        private async Task<string> GetMonth()
        {
            string message = BASE_MESSAGE + "Month Interval\n" + CRON_GUIDE + "\n\n";

            var embed = CurrentConfig.BuildEmbed(InvokingUser)
                .WithDescription(message)
                .AddFields(EnumHelper.ToMenuDictionary(typeof(Month), false));

            string reply = await RetrieveData<string>(embed, true);

            while (!reply.IsValidCron(1, 12))
            {
                TrackedMessages.Add(await Context.ReplyWithStatus(StatusCode.WARNING, "Invalid cron string.", InvokingUser));
                reply = await RetrieveData<string>(embed, true);
            }

            return reply;
        }

        private async Task<string> GetRange(string customMessage, int min, int max)
        {
            string display = BASE_MESSAGE + customMessage + "\n" + CRON_GUIDE + "\n\n" + $"Valid range: {min} - {max}";

            var embed = CurrentConfig.BuildEmbed(InvokingUser)
                .WithDescription(display);

            string value = await RetrieveData<string>(embed, true);

            while (!value.IsValidCron(min, max))
            {
                TrackedMessages.Add(await Context.ReplyWithStatus(StatusCode.WARNING, "Invalid cron string", InvokingUser));
                value = await RetrieveData<string>(embed, true);
            }

            return value;
        }
    }
}
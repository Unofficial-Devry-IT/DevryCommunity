﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Discord.SlashCommands.Filters;
using DevryBot.Options;
using DevryDomain.Models;
using DevryInfrastructure.Persistence;
using DisCatSharp.Entities;
using DisCatSharp.SlashCommands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UnofficialDevryIT.Architecture.Extensions;
using UnofficialDevryIT.Architecture.Scheduler;

namespace DevryBot.Discord.SlashCommands.Reminders
{
    public class CreateReminder : SlashCommandModule
    {
        public IOptions<DiscordOptions> DiscordOptions { get; set; }
        public ILogger<CreateReminder> Logger { get; set; }
        public IScheduledTaskService ScheduleService { get; set; }
        public IApplicationDbContext Context { get; set; }

        [SlashCommand("create-reminder", "Create a reoccurring message")]
        [RequireModerator]
        public async Task Command(InteractionContext context,
            [Option("Title", "What is this reminder about?")]
            string title,
            [Option("DayOfWeek", "Cronjob syntax for selecting day of week (SUN - SAT).")]
            string dayOfWeekText,
            [Option("Month", "Cronjob syntax for selecting month.")]
            string monthText,
            [Option("Day", "Cronjob syntax for selecting day.")]
            string dayText,
            [Option("Hours", "Cronjob syntax for selecting hours.")]
            string hoursText,
            [Option("Minutes", "Cronjob syntax for selecting minutes.")]
            string minutesText)
        {
            if (!await context.ValidateGuild())
                return;
        
            await context.ImThinking();

            DiscordWebhookBuilder responseBuilder = new();
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Scheduler")
                .WithFooter("Reminder Creation");

            if (ScheduleService == null)
            {
                embedBuilder.Description = "Reminder Background Service is unavailable";
                embedBuilder.Color = DiscordColor.Red;

                responseBuilder.AddEmbed(embedBuilder.Build());
                await context.EditResponseAsync(responseBuilder);
                return;
            }
            
            Dictionary<string, string> errors = new();

            if (!dayOfWeekText.IsValidCron(0, 6))
                errors.Add("Day of Week", "Invalid day of week syntax");

            if (!monthText.IsValidCron(1, 12))
                errors.Add("Month", "Invalid syntax for month");

            if (!dayText.IsValidCron(1, 31))
                errors.Add("Day", "Invalid syntax for day");

            if (!hoursText.IsValidCron(0, 23))
                errors.Add("Hours", "Invalid syntax for hours");

            if (!minutesText.IsValidCron(0, 59))
                errors.Add("Minutes", "Invalid syntax for minutes");

            // Provide feedback as to what the errors are (if any) -- stop processing if there are errors
            if (errors.Any())
            {
                embedBuilder.Description =
                    "Please check out the following site to help create cron-strings: https://crontab.guru/";
                embedBuilder.ImageUrl = DiscordOptions.Value.ErrorImage;
                embedBuilder.Color = DiscordColor.Red;
                
                foreach (var pair in errors)
                {
                    Logger.LogWarning(
                        $"{context.Member.Username} -- error with reminder {pair.Key} | {pair.Value}");
                    
                    embedBuilder.AddField(pair.Key, pair.Value);
                }

                responseBuilder.AddEmbed(embedBuilder.Build());
                await context.EditResponseAsync(responseBuilder);
                return;
            }

            string schedule = $"{minutesText} {hoursText} {dayText} {monthText} {dayOfWeekText}";

            Reminder reminder = new()
            {
                ChannelId = context.Channel.Id,
                Contents = CronExpressionDescriptor.ExpressionDescriptor.GetDescription(schedule),
                Name = title,
                Schedule = schedule
            };

            try
            {
                await ScheduleService.AddTask(reminder);
                await Context.Reminders.AddAsync(reminder);
                await Context.SaveChangesAsync(default);
                
                embedBuilder.Color = DiscordColor.Green;
                embedBuilder.Description = "Successfully created reminder with the following schedule:\n" +
                                           CronExpressionDescriptor.ExpressionDescriptor.GetDescription(
                                               reminder.Schedule);
                embedBuilder.ImageUrl = DiscordOptions.Value.CompletedImage;
            }
            catch (Exception ex)
            {
                embedBuilder.Color = DiscordColor.Red;
                embedBuilder.Description = $"An error occurred while trying to save the reminder. {ex.Message}";
                embedBuilder.ImageUrl = DiscordOptions.Value.ErrorImage;
            }

            responseBuilder.AddEmbed(embedBuilder.Build());
            await context.EditResponseAsync(responseBuilder);

        }
    }
}
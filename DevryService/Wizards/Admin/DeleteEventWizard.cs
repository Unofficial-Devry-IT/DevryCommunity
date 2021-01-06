using DevryService.Core;
using DevryService.Core.Schedule;
using DevryService.Models;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Wizards.Admin
{
    public class DeleteEventWizardConfig : CreateEventWizardConfig
    {

    }

    public class DeleteEventWizard : WizardBase<DeleteEventWizardConfig>
    {
        public DeleteEventWizard(CommandContext commandContext) : base(commandContext)
        {
        }

        public override DeleteEventWizardConfig DefaultSettings()
        {
            DeleteEventWizardConfig config = new DeleteEventWizardConfig();

            config.Name = "Procrastination Hat";
            config.Title = "Let's cancel that event!";
            config.Icon = "";
            config.Description = "Allows user to delete an event";

            config.AllowedToUse = new List<string>()
            {
                "Moderator",
                "Tutor",
                "Professor"
            };

            return config;
        }

        protected override async Task ExecuteAsync(CommandContext context)
        {
            List<Reminder> reminders = await Bot.Instance.DiscordService.GetReminders((x) => x.ChannelId == context.Channel.Id);

            if(reminders.Count == 0)
            {
                await SimpleReply(context, "No reminders are set for this channel...", false, false);
                return;
            }

            string baseMessage = "Please select the corresponding number(s) to delete a reminder\n";

            for (int i = 0; i < reminders.Count; i++)
                baseMessage += $"[{i + 1}] - {reminders[i].Name}\t{CronExpressionDescriptor.ExpressionDescriptor.GetDescription(reminders[i].Schedule)}";

            string reply = string.Empty;
            
            _recentMessage = await WithReply(context, baseMessage, (context) => reply = context.Result.Content, true);

            string[] parameters = reply.Replace(",", " ").Split(" ");
            List<string> removed = new List<string>();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    index -= 1;

                    if(index < 0 || index >= reminders.Count)
                    {
                        _recentMessage = await ReplyEdit(_recentMessage, $"Invalid Range. Valid Range: 1 - {reminders.Count}", true, false);
                        continue;
                    }

                    SchedulerBackgroundService.Instance.RemoveTask(reminders[index].Id);
                    await Bot.Instance.DiscordService.DeleteReminder(reminders[index].Id);
                    removed.Add($"{reminders[index].Name} with Id {reminders[index].Id}");
                }
            }

            if (removed.Count > 0)
                await SimpleReply(context, $"Following events were removed: {string.Join("\n", removed)}", false, false);
            else
                await SimpleReply(context, $"No changes were made...", false, false);
        }
    }
}

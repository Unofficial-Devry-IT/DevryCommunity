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

        const string AUTHOR_NAME = "Procrastination Hat";
        const string DESCRIPTION = "Allows user to delete an event";
        const string REACTION_EMOJI = "";
        const string AUTHOR_ICON = "";
        readonly List<string> RESTRICTED_ROLES = new List<string>()
        {
            "Moderator",
            "Tutor",
            "Professor"
        };

        public override CommandConfig DefaultCommandConfig()
        {
            return new CommandConfig
            {
                AuthorName = AUTHOR_NAME,
                Description = DESCRIPTION,
                ReactionEmoji = REACTION_EMOJI,
                IgnoreHelpWizard = true,
                RestrictedRoles = RESTRICTED_ROLES
            };
        }
     
        public override DeleteEventWizardConfig DefaultSettings()
        {
            DeleteEventWizardConfig config = new DeleteEventWizardConfig();

            config.AuthorName = AUTHOR_NAME;
            config.Headline = "Let's cancel that event!";
            config.AuthorIcon = AUTHOR_ICON;
            config.Description = DESCRIPTION;
            config.AllowedToUse = RESTRICTED_ROLES;

            config.UsesCommand = new WizardToCommandLink
            {
                CommandConfig = DefaultCommandConfig(),
                DiscordCommand = "delete-event"
            };

            return config;
        }

        protected override async Task ExecuteAsync(CommandContext context)
        {
            // TODO: Why is the predicate overload not working the way it should
            List<Reminder> reminders = await Worker.Instance.DiscordService.GetReminders();
            reminders = reminders.Where(x => x.ChannelId == context.Channel.Id).ToList();

            if(reminders.Count == 0)
            {
                await SimpleReply(context, "No reminders are set for this channel...", false, false);
                return;
            }

            string baseMessage = "Please select the corresponding number(s) to delete a reminder\n";

            var embed = EmbedBuilder()
                .WithDescription(baseMessage);

            for (int i = 0; i < reminders.Count; i++)
                embed.AddField((i + 1).ToString(),
                    $"{reminders[i].Name}\t{CronExpressionDescriptor.ExpressionDescriptor.GetDescription(reminders[i].Schedule)}", true);

            string reply = string.Empty;

            _recentMessage = await WithReply(context, embed.Build(), (context) => ReplyHandlerAction(context, ref reply), true);

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
                    await Worker.Instance.DiscordService.DeleteReminder(reminders[index].Id);
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

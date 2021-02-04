using DevryService.Bot.Exceptions;
using DevryService.Database.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryService.Bot.Wizards.Admin
{
    public class DeleteEventWizard : WizardBase
    {
        class ExtensionConfig
        {
            public List<string> RestrictedRoles = new List<string>()
            {
                "Moderator",
                "Tutor",
                "Professor"
            };
        }
        public DeleteEventWizard(CommandContext context) : base(context)
        {
        }

        List<Reminder> reminders = new List<Reminder>();

        protected override void Initialize()
        {
            base.Initialize();

            if(Database != null)
            {
                reminders = Database.Reminders
                    .Where(x=>x.ChannelId == Context.Channel.Id && x.GuildId == Context.Guild.Id)
                    .ToList();
            }
        }

        protected override async Task ExecuteAsync()
        {
            if(reminders.Count == 0)
            {
                await SimpleReply(BasicEmbed().WithDescription("No reminders are set for this channel...").WithColor(DiscordColor.Red).Build(), false);
                throw new StopWizardException(AuthorName);
            }

            string baseMessage = "Please select the corresponding number(s) to delete a reminder\n";
            DiscordEmbedBuilder builder = BasicEmbed().WithDescription(baseMessage);
            var primaryMessage = await WithReply<string>(builder.Build());

            for (int i = 0; i < reminders.Count; i++)
                builder.AddField((i + 1).ToString(),
                    $"{reminders[i].Name}\t{CronExpressionDescriptor.ExpressionDescriptor.GetDescription(reminders[i].Schedule)}", true);

           if(primaryMessage.message == null)
                throw new StopWizardException(AuthorName);

            string[] parameters = primaryMessage.value.Replace(",", " ").Split(" ");
            List<string> removed = new List<string>();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    index -= 1;

                    if(index < 0 || index >= reminders.Count)
                        continue;


                    removed.Add($"{reminders[index].Name} with Id {reminders[index].Id}");
                }
            }
        }

        protected override string GetDefaultAuthorIcon() => "";

        protected override string GetDefaultAuthorName() => "Procrastination Hat";

        protected override string GetDefaultDescription() => "Allows a user to delete an event";

        protected override string GetDefaultHeadline() => "Let's cancel that event!";

        protected override TimeSpan? GetDefaultTimeoutOverride() => null;

        protected override string GetDefaultExtensionData() => Newtonsoft.Json.JsonConvert.SerializeObject(new ExtensionConfig());
        
    }
}

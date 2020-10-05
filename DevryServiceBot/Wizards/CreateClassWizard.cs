using DevryServiceBot.Util;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using System.ComponentModel.DataAnnotations;

namespace DevryServiceBot.Wizards
{
    [WizardInfo(Name ="Enrollment Wizard", Description = "Add a new class to the community", Group = "Utility", IgnoreHelpWizard=true)]
    public class CreateClassWizard : Wizard
    {
        public CreateClassWizard(CommandContext context) : base(context.User.Id, context.Channel) { }

        public override async Task StartWizard(CommandContext context)
        {
            const string basic = "Create Class Wizard. Please follow the Instructions below\n";

            string classTitle = "",
                   classRoleName = "";

            int numberOfVoiceChannels = 3;

            DiscordMessage main = await WizardReply(context, basic + "\nWhat is the Course Identifier?", true);

            DiscordMessage reply = await GetUserReply();

            classRoleName = reply.Content;

            main = await WizardReply(context, basic + "\nWhat's the course title? A human friendly name?");

            reply = await GetUserReply();
            classTitle = reply.Content;


            var category = await context.Guild.CreateChannelAsync(classRoleName + "-" + classTitle, DSharpPlus.ChannelType.Category);            
        }
    }
}

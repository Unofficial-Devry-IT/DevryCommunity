using DevryServiceBot.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevryServiceBot.Commands.General
{
    public class QuestionaireCommands
    {
        [Command("questionaire")]
        public async Task StartQuestionaire(CommandContext context)
        {
            QuestionaireWizard wizard = new QuestionaireWizard(context.User.Id, context.Channel);
            await wizard.StartWizard(context);
        }
    }
}

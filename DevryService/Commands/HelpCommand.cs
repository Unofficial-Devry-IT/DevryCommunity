using DevryService.Core;
using DevryService.Core.Util;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Description = DevryService.Core.Util.DescriptionAttribute;

namespace DevryService.Commands
{

    public class HelpCommand : IDiscordCommand, IMiscCommand
    {
        [Command("view-welcome")]
        [WizardCommandInfo(Description = "View the message that gets sent to newcomers when they join!")]
        public async Task ViewWelcome(CommandContext context)
        {
            await context.Member.SendMessageAsync(embed: Bot.GenerateWelcomeMessage());
        }

        [Command("invite")]
        [WizardCommandInfo(Description = "Spread the word! Get your fellow classmates to join us!",
            Emoji = ":email:",
            IgnoreHelpWizard = false,
            Name = "Invitation Link")]
        public async Task InviteLink(CommandContext context)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor("Recruiter Hat", icon_url: "")
                .WithTitle("Invitation")
                .WithColor(DiscordColor.Cyan)
                .WithDescription("Spread the word, our trusted scout! Spread the word " +
                "of our kingdom! Amass an army of knowledge seeking minions! Lay " +
                "waste to the legions of doubt and uncertainty!!")
                .AddField("Invite", "https://discord.gg/P5BDakb")
                .WithFooter("Minions of knowledge! Assembblleeeee!");

            await context.RespondAsync(embed: builder.Build());
        }

        [Command("stats")]
        [WizardCommandInfo(Description = "Not fully implemented yet")]
        public async Task ViewStats(CommandContext context)
        {
            using (DevryDbContext database = new DevryDbContext())
            {
                var stats = await database.Stats.FindAsync(context.Member.Id);

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                    .WithAuthor("Stats Hat", icon_url: "https://alifeofproductivity.com/wp-content/uploads/2013/06/stat.001.jpg")
                    .WithColor(DiscordColor.Gray)
                    .WithTitle($"{context.Member.Username} Stats");

                if (stats != null)
                    builder.AddField("Points", stats.Points.ToString());
                else
                    builder.AddField("Points", "0");

                await context.RespondAsync(embed: builder.Build());
            }
        }

        [Command("help")]
        [WizardCommandInfo(Description = "A wizard shall appear and guide you along")]
        public async Task Help(CommandContext context)
        {
            HelpWizard wizard = new HelpWizard(context.Member.Id, context.Channel);
            try
            {
                await wizard.StartWizard(context);
            }
            catch(StopWizardException ex)
            {
                await wizard.Cleanup();
            }
            catch(Exception ex)
            {
                await wizard.Cleanup();
            }
        }
    }
}

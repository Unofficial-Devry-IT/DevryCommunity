using DevryServiceBot.Util;
using DevryServiceBot.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryServiceBot.Commands.General
{
    public class HelpCommands
    {
        [Command("view-welcome")]
        public async Task ViewWelcome(CommandContext context)
        {
            await context.Member.SendMessageAsync(embed: Bot.GenerateWelcomeMessage());
        }

        [Command("invite")]
        public async Task InivteLink(CommandContext context)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor("Recruiter Hat", icon_url: "https://media.sproutsocial.com/uploads/2019/09/Employee-Retention.png")
                .WithTitle("Invitation")
                .WithColor(DiscordColor.Cyan)
                .WithDescription("https://discord.gg/P5BDakb")
                .WithFooter("Minions of knowledge! Assembbbleeeeeee!!");

            await context.RespondAsync(embed: builder.Build());
        }

        [Command("stats")]
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
        public async Task Help(CommandContext context)
        {
            HelpWizard wizard = new HelpWizard(context);

            try
            {
                await wizard.StartWizard(context);
            }
            catch (StopWizardException ex)
            {
                // user, or something prompted the user to stop
                Console.WriteLine(ex.Message);
                await wizard.Cleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                await wizard.Cleanup();
            }
        }

        [Command("test")]
        public async Task Test(CommandContext context)
        {
            MessageContext messageContext = await Bot.Interactivity.WaitForMessageAsync((message) =>
            {
                return message.Author.Id == context.User.Id && message.ChannelId == context.Channel.Id;
            });

            DiscordMessage message = await context.RespondAsync("I received your message: " + messageContext.Message.Content);

            ReactionContext reaction = await Bot.Interactivity.WaitForMessageReactionAsync(message, context.User);

            await message.RespondAsync($"You reactedwith \t {reaction.Emoji.Name}");
        }
    }
}

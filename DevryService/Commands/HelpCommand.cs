using DevryService.Core;
using DevryService.Core.Util;
using DevryService.Wizards;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Description = DevryService.Core.Util.DescriptionAttribute;

namespace DevryService.Commands
{
    
    public class HelpCommand : BaseCommandModule, IDiscordCommand, IMiscCommand
    {
        [Command("view-welcome")]
        [Settings("viewWelcomeConfig")]
        public async Task ViewWelcome(CommandContext context)
        {
            DiscordChannel test = Bot.Discord.Guilds
               .FirstOrDefault(x=>x.Value.Name.ToLower().Contains("devry")).Value.Channels
               .FirstOrDefault(x => x.Value.Name.ToLower().Contains("bot-test"))
               .Value;

            await test.SendMessageAsync(embed: Bot.GenerateWelcomeMessage(context.Member));
        }

        [Command("invite")]
        [Settings("inviteLinkConfig")]
        public async Task InviteLink(CommandContext context)
        {
            EmbedConfig config = CommandSettingsUtil.InviteLinkConfig();

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithAuthor(config.Name, iconUrl: "")
                .WithTitle(config.Title)
                .WithColor(DiscordColor.Cyan)
                .WithDescription(config.Contents)                
                .WithFooter(config.Footer);

            foreach(var field in config.Fields)
            {
                string[] split = field.Split("|");

                if (split.Length != 2) continue;

                builder.AddField(split[0], split[1]);
            }

            await context.RespondAsync(embed: builder.Build());
        }

        [Command("stats")]
        [Settings("viewStatsConfig")]
        public async Task ViewStats(CommandContext context)
        {
            CommandConfig config = CommandSettingsUtil.ViewStatsConfig();

            using (DevryDbContext database = new DevryDbContext())
            {
                var stats = await database.Stats.FindAsync(context.Member.Id);

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                    .WithAuthor(config.Name, iconUrl: config.Icon)
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
        [Settings("helpCommandConfig")]
        [WizardCommandInfo(Description = "A wizard shall appear and guide you along")]
        public async Task Help(CommandContext context)
        {
            HelpWizard wizard = new HelpWizard(context);

            try
            {
                wizard.Run(context);
            }
            catch(Exception)
            {
                await wizard.CleanupAsync();
            }
        }
    }
}

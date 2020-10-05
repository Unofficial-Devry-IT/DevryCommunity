using DevryServiceBot.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryServiceBot.Wizards
{
    [WizardInfo(Name ="Bouncer Hat",
                Description = "Need to leave a class? Let's get ya outta there!",
                IconUrl = "https://vignette.wikia.nocookie.net/harrypotter/images/6/62/Sorting_Hat.png/revision/latest?cb=20161120072849",
                Group = "Role",
                ReactionEmoji = ":classical_building:",
                CommandName = "RoleCommand.LeaveRole")]
    public class LeaveRoleWizard : Wizard
    {
        public LeaveRoleWizard(CommandContext context) : base(context.User.Id, context.Channel) { }

        public override async Task StartWizard(CommandContext context)
        {
            var member = await context.Guild.GetMemberAsync(UserId);
            var roles = member.Roles.Where(x => NotBlacklisted(x.Name))
                .OrderBy(x=>x.Name)
                .ToList();

            if(roles.Count == 0)
            {
                await WizardReply(context, "You don't have any roles for me to remove!", false);
                return;
            }

            string message = "Selected the number(s) that correspond to the role you wish to remove\n";
            for(int i = 0; i < roles.Count; i++)
            {               
                message += $"[{i + 1}] - {roles[i].Name}\n";
            }

            await WizardReply(context, message, true);

            DiscordMessage userReply = await GetUserReply();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var parameters = userReply.Content.Replace(",", " ").Split(" ");
            List<string> removed = new List<string>();
            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    await member.RevokeRoleAsync(roles[index - 1]);
                    removed.Add(roles[index - 1].Name);
                }
            }

            await Cleanup();

            if (removed.Count > 0)
            {
                await WizardReply(context, $"The following roles were removed: {string.Join(", ", removed)}", false);
            }
            else
            {
                await WizardReply(context, "No changes were made", false);
            }
        }
    }
}

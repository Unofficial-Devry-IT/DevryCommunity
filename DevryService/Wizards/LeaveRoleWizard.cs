using DevryService.Core;
using DevryService.Core.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Wizards
{
    [WizardInfo(Name ="Bouncer Hat",
        Title = "Leave Class(es)",
        IconUrl = "https://vignette.wikia.nocookie.net/harrypotter/images/6/62/Sorting_Hat.png/revision/latest?cb=20161120072849",
        Description = "Select the corresponding number(s) to leave a class")]
    public class LeaveRoleWizard : Wizard
    {
        List<DiscordRole> Roles;
        DiscordMessage WizardMessage;

        public LeaveRoleWizard(ulong userId, DiscordChannel channel) : base(userId, channel)
        {
            
        }

        public override async Task StartWizard(CommandContext context)
        {
            var member = await context.Guild.GetMemberAsync(OriginalUserId);
            Roles = member.Roles.Where(x => NotBlacklisted(x.Name))
                .OrderBy(x => x.Name)
                .ToList();

           if(Roles.Count == 0)
            {
                await WizardReply(context, "You don't have any roles for me to remove!", false);
                return;
            }

            string message = "Select the number(s) that correspond to the role you wish to remove\n";

            for (int i = 0; i < Roles.Count; i++)
                message += $"[{i + 1}] - {Roles[i].Name}\n";

            await WizardReply(context, message, true);
            DiscordMessage reply = await GetUserReply();

            var parameters = reply.Content.Replace(",", " ").Split(" ");
            List<string> removed = new List<string>();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    await member.RevokeRoleAsync(Roles[index - 1]);
                    removed.Add(Roles[index - 1].Name);
                }
            }

            await Cleanup();

            if (removed.Count > 0)
                await WizardReply(context, $"The following roles were removed: {string.Join(", ", removed)}", false);
            else
                await WizardReply(context, "No changes were made", false);
        }
    }
}

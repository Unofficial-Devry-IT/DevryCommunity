using DevryService.Core;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using DevryService.Core.Util;

namespace DevryService.Wizards
{
    [WizardInfo(Name ="Sorting Hat",
        IconUrl = "https://vignette.wikia.nocookie.net/harrypotter/images/6/62/Sorting_Hat.png/revision/latest?cb=20161120072849",
        Title = "Let's get you settled")]
    public class JoinRoleWizard : Wizard
    {
        public JoinRoleWizard(ulong userId, DiscordChannel channel) : base(userId, channel)
        {
        }

        public override async Task StartWizard(CommandContext context)
        {
            var roles = context.Guild.Roles
                .Where(x => NotBlacklisted(x.Name))
                .OrderBy(x => x.Name)
                .ToList();

            List<string> courseTypes = roles.Select(x => x.Name.Replace("-", " ").Split(" ").First())
                .Distinct()
                .ToList();

            string message = $"Which course(s) are you currently attending/teaching? Below is a list of categories. \nPlease type in the number(s) associated with the course\n";

            for (int i = 0; i < courseTypes.Count; i++)
                message += $"[{i + 1}] - {courseTypes[i]}\n";

            await WizardReply(context, message, true);

            DiscordMessage userReply = await GetUserReply();

            var parameters = userReply.Content.Replace(",", " ").Split(" ");
            Dictionary<string, List<DiscordRole>> selectedGroups = new Dictionary<string, List<DiscordRole>>();
            Dictionary<int, DiscordRole> roleMap = new Dictionary<int, DiscordRole>();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    index -= 1;

                    if (index < 0 || index > courseTypes.Count)
                    {
                        Console.WriteLine($"Invalid input");
                    }
                    else
                        selectedGroups.Add(courseTypes[index], roles.Where(x => x.Name.ToLower().StartsWith(courseTypes[index].ToLower())).ToList());
                }
            }

            int current = 0;
            foreach(var key in selectedGroups.Keys)
            {
                message = $"Select the number associated with the class(es) you wish to join\n\n{key}:\n";
                foreach(var item in selectedGroups[key])
                {
                    message += $"[{current + 1}] - {item.Name}\n";
                    roleMap.Add(current, item);
                    current++;
                }

                await WizardReply(context, message, true);
            }

            userReply = await GetUserReply();
            parameters = userReply.Content.Replace(",", " ").Split(" ");
            List<string> appliedRoles = new List<string>();

            DiscordMember member = await context.Guild.GetMemberAsync(OriginalUserId);

            foreach (var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    index -= 1;

                    if (index < 0 || index >= roleMap.Count)
                        Console.WriteLine($"Invalid Input: {index + 1}");
                    else
                    {
                        await context.Guild.GrantRoleAsync(member, roleMap[index]);
                        appliedRoles.Add(roleMap[index].Name);
                    }
                }
            }

            await Cleanup();

            if (appliedRoles.Count > 0)
                await WizardReply(context, $"Hey, {context.Member.Username}, the following roles were applied: \n{string.Join(", ", appliedRoles)}", false);
            else
                await WizardReply(context, $"Hey, {context.Member.Username}, no changes were applied", false);
        }
    }
}

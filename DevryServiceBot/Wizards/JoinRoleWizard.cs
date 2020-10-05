using DevryServiceBot.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace DevryServiceBot.Wizards
{
    [WizardInfo(Name ="Sorting Hat",
                IconUrl = "https://vignette.wikia.nocookie.net/harrypotter/images/6/62/Sorting_Hat.png/revision/latest?cb=20161120072849",
                Description = "Let's get you settled into your class(es)!",
                Group = "Role",
                ReactionEmoji = ":classical_building:",
                GroupDescription = "Join/Leave class(es)",
                CommandName = "RoleCommand.JoinRole")]
    public class JoinRoleWizard : Wizard
    {
        public JoinRoleWizard(CommandContext context) : base(context.User.Id, context.Channel){}

        public override async Task StartWizard(CommandContext context)
        {
            // We need to grab all the roles available on our server!
            var roles = context.Guild.Roles
                            .Where(x => NotBlacklisted(x.Name))
                            .OrderBy(x=>x.Name)
                            .ToList();
            
            List<string> courseTypes = roles.Select(x => x.Name.Replace("-", " ").Split(" ").First())
                .Distinct()
                .ToList();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Prompt the user to select the course(s) they're attending or teaching. Get their selection for processing

            string message = $"Which course(s) are you currently attending/teaching? Below is a list of categories. \nPlease type in the number(s) associated with the course\n";

            for (int i = 0; i < courseTypes.Count; i++)
                message += $"[{i + 1}] - {courseTypes[i]}\n";

            await WizardReply(context, message, true);

            DiscordMessage userReply = await GetUserReply();

            var parameters = userReply.Content.Replace(',', ' ').Split(' ');
            Dictionary<string, List<DiscordRole>> SelectedGroups = new Dictionary<string, List<DiscordRole>>();
            Dictionary<int, DiscordRole> roleMap = new Dictionary<int, DiscordRole>();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    index -= 1; // We gave the user the 1 based approach. Need to convert to 0 based

                    if (index < 0 || index > courseTypes.Count)
                    {
                        await AddError(userReply, $"Invalid Input: {index+1}"); // need to make sure we show them what they did wrong
                    }
                    else
                        SelectedGroups.Add(courseTypes[index], roles.Where(x=>x.Name
                                                                                .ToLower()
                                                                                .StartsWith(courseTypes[index].ToLower()))
                                                                    .ToList());
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Based on user input... let's provide the next prompt which will list out the full course name

            int current = 0;

            foreach(var key in SelectedGroups.Keys)
            {
                message = $"Select the number associated with the class(es) you wish to join.\n\n{key}:\n";
                foreach(var item in SelectedGroups[key])
                {
                    message += $"[{current + 1}] - {item.Name}\n";
                    roleMap.Add(current, item);
                    current++;
                }

                await WizardReply(context, message, true);
            }

            // Get user reply
            userReply = await GetUserReply();

            parameters = userReply.Content.Replace(",", " ").Split(" ");

            List<string> appliedRoles = new List<string>();
            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    index -= 1;

                    if(index < 0 || index >= roleMap.Count)
                    {
                        await AddError(userReply, $"Invalid Input: {index + 1}");
                    }
                    else
                    {
                        DiscordMember member = await context.Guild.GetMemberAsync(UserId);
                        await context.Guild.GrantRoleAsync(member, roleMap[index]);
                        appliedRoles.Add(roleMap[index].Name);
                    }
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            await Cleanup();

            if (appliedRoles.Count > 0)
            {
               await WizardReply(context, $"Hey, {context.Member.Username}, the following roles were applied: \n{string.Join(", ", appliedRoles)}", false);
            }
            else
            {
                await WizardReply(context, $"Hey, {context.Member.Username}, No changes were applied", false);
            }
        }
    }
}

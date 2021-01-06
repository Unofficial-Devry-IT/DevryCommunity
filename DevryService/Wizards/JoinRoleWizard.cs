using DevryService.Core;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevryService.Core.Util;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;

namespace DevryService.Wizards
{
    public class JoinRoleWizardConfig : WizardConfig
    {
        public List<string> BlacklistedRoles = new List<string>()
        {

        };
    }

    public class JoinRoleWizard : WizardBase<JoinRoleWizardConfig>
    {
        public override JoinRoleWizardConfig DefaultSettings()
        {
            JoinRoleWizardConfig config = new JoinRoleWizardConfig();

            config.Icon = "https://vignette.wikia.nocookie.net/harrypotter/images/6/62/Sorting_Hat.png/revision/latest?cb=20161120072849";
            config.Name = "Sorting Hat";
            config.Title = "Let's get you settled";

            config.BlacklistedRoles = new List<string>()
            {

            };

            return config;
        }

        public JoinRoleWizard(CommandContext commandContext) : base(commandContext)
        {
        }

        protected override async Task ExecuteAsync(CommandContext context)
        {
            var roles = context.Guild.Roles
                .Where(x => !_options.BlacklistedRoles.Contains(x.Value.Name))
                .OrderBy(x => x.Value.Name)
                .Select(x=>x.Value)
                .ToList();

            List<string> courseTypes = roles.Select(x => x.Name.Replace("-", " ").Split(" ").First())
                .Distinct()
                .ToList();

            string message = $"Which course(s) are you currently attending/teaching? Below is a list of categories. \nPlease type in the number(s) associated with the course\n";

            for (int i = 0; i < courseTypes.Count; i++)
                message += $"[{i + 1}] - {courseTypes[i]}\n";

            string reply = string.Empty;
            _recentMessage = await WithReply(context, message, (context) => reply = context.Result.Content, true);
            string[] parameters = reply.Replace(",", " ").Split(" ");

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

                await SimpleReply(context, message, true, true);
            }

            reply = string.Empty;
            var response = await _recentMessage.GetNextMessageAsync();

            if (response.TimedOut)
            {
                await SimpleReply(context, $"{_options.Name} Wizard timed out...", false, false);
                throw new StopWizardException(_options.Name);
            }

            _messages.Add(response.Result);

            reply = response.Result.Content.Trim();

            try
            {
                parameters = reply.Replace(",", " ").Split(" ");
            }
            catch
            {
                await CleanupAsync();
                return;
            }

            List<string> appliedRoles = new List<string>();

            DiscordMember member = await context.Guild.GetMemberAsync(_originalMember.Id);

            foreach (var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    index -= 1;

                    if (index < 0 || index >= roleMap.Count)
                        Console.WriteLine($"Invalid Input: {index + 1}");
                    else
                    {
                        await _originalMember.GrantRoleAsync(roleMap[index]);
                        appliedRoles.Add(roleMap[index].Name);
                    }
                }
            }

            await CleanupAsync();

            if (appliedRoles.Count > 0)
                await SimpleReply(context, $"Hey, {context.Member.Username}, the following roles were applied: \n{string.Join(", ", appliedRoles)}", false, false);
            else
                await SimpleReply(context, $"Hey, {context.Member.Username}, no changes were applied", false, false);
        }
    }
}

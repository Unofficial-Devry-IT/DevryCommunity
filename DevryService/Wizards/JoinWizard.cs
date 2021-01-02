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
    [WizardInfo(Name = "Sorting Hat",
        IconUrl = "",
        Title ="Let's get you settled!")]
    public class JoinWizard : BaseWizard
    {
        public JoinWizard(CommandContext context) : base(context)
        {
        }

        public override async Task StartWizard()
        {
            var roles = this.context.Guild.Roles
                .Where(x => NotBlacklisted(x.Name))
                .OrderBy(x => x.Name)
                .ToList();

            List<string> courseTypes = roles.Select(x => x.Name.Replace("-", " ").Split(" ").First())
                .Distinct()
                .ToList();

            string message = $"Which course(s) are you currently attending/teaching? Below is a list of categories.\nPlease type in the number(s) associated with the course(s)\n{CreateOptionList(courseTypes)}";

            await SendMessage(message);

            var results = await GetUserReplyOptions(courseTypes);

            if(!results.success)
            {
                Console.WriteLine($"Something went wrong while trying to pick course categories");
                throw new StopWizardException(WizardName);
            }

            Dictionary<string, List<DiscordRole>> selectedGroups = new Dictionary<string, List<DiscordRole>>();
            Dictionary<int, DiscordRole> roleMap = new Dictionary<int, DiscordRole>();

            foreach(var option in results.options)
            {
                
            }
        }
    }
}

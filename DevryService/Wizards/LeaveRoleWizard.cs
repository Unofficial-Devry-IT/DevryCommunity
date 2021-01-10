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
    public class LeaveRoleWizardConfig : JoinRoleWizardConfig
    {

    }

    [WizardInfo(Name ="Bouncer Hat",
        Title = "Leave Class(es)",
        IconUrl = "",
        Description = "Select the corresponding number(s) to leave a class")]
    public class LeaveRoleWizard : WizardBase<LeaveRoleWizardConfig>
    {
        public LeaveRoleWizard(CommandContext commandContext) : base(commandContext)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        const string AUTHOR_NAME = "Bouncer Hat";
        const string DESCRIPTION = "Select the corresponding number(s) to leave a class";
        const string REACTION_EMOJI = "";
        const string AUTHOR_ICON = "https://vignette.wikia.nocookie.net/harrypotter/images/6/62/Sorting_Hat.png/revision/latest?cb=20161120072849";

        public override CommandConfig DefaultCommandConfig()
        {
            return new CommandConfig
            {
                AuthorName = AUTHOR_NAME,
                Description = DESCRIPTION,
                ReactionEmoji = REACTION_EMOJI,
                AuthorIcon = AUTHOR_ICON,
                IgnoreHelpWizard = false
            };
        }

        public override LeaveRoleWizardConfig DefaultSettings()
        {
            LeaveRoleWizardConfig config = new LeaveRoleWizardConfig();

            config.AuthorName = AUTHOR_NAME;
            config.Headline = "Leave Class(es)";
            config.Description = DESCRIPTION;
            config.AuthorIcon = AUTHOR_ICON;

            config.MessageRequireMention = false;
            config.AcceptAnyUser = false;
            config.BlacklistedRoles = new string[]
            {
                "Moderator", "Admin"
            };

            config.UsesCommand = new WizardToCommandLink
            {
                DiscordCommand = "leave",
                CommandConfig = DefaultCommandConfig()
            };

            return config;
        }

        protected override async Task ExecuteAsync(CommandContext context)
        {
            var roles = _channel.Guild.Roles
                .Where(x => !_options.BlacklistedRoles.Contains(x.Value.Name))
                .OrderBy(x => x.Value.Name)
                .Select(x => x.Value)
                .ToList();

            var member = await context.Guild.GetMemberAsync(_originalMember.Id);

            var memberRoles = member.Roles.Where(x => !_options.BlacklistedRoles.Contains(x.Name))
                .OrderBy(x => x.Name)
                .ToList();

            if(memberRoles.Count == 0)
            {
                await SimpleReply(context, "You don't have any roles for me to remove!", false, false);
                return;
            }

            string message = "Select the number(s) that correspond to the role you wish to remove\n";

            for (int i = 0; i < memberRoles.Count; i++)
                message += $"[{i + 1}] - {memberRoles[i].Name}\n";

            string reply = string.Empty;
            _recentMessage = await WithReply(context, message, (context) => reply = context.Result.Content, true);

            string[] parameters = null;

            try
            {
                parameters = reply.Replace(",", " ").Split(" ");
            }
            catch
            {
                await CleanupAsync();
                return;
            }

            List<string> removed = new List<string>();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    await member.RevokeRoleAsync(memberRoles[index - 1]);
                    removed.Add(memberRoles[index - 1].Name);
                }
            }

            await CleanupAsync();

            if (removed.Count > 0)
                await SimpleReply(context, $"The following roles were removed: {string.Join(", ", removed)}", false, false);
            else
                await SimpleReply(context, "No changes were made", false, false);
        }
    }
}

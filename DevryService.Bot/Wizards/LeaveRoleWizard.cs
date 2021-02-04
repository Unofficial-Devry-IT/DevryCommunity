using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryService.Bot.Wizards
{
    public class LeaveRoleWizard : WizardBase
    {
        class ExtensionConfig
        {
            public List<string> BlacklistedRoles = new List<string>();
        }

        private ExtensionConfig _extensionConfig;

        public LeaveRoleWizard(CommandContext context) : base(context)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            _extensionConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtensionConfig>(ExtensionData);
        }

        protected override async Task ExecuteAsync()
        {
            var lowercased = _extensionConfig.BlacklistedRoles.Select(x => x.ToLower()).ToList();

            var roles = Context.Guild.Roles
                .Where(x => !lowercased.Contains(x.Value.Name.ToLower()))
                .OrderBy(x => x.Value.Name)
                .Select(x => x.Value)
                .ToList();

            var memberRoles = Context.Member.Roles.Where(x => !lowercased.Contains(x.Name.ToLower()))
                .OrderBy(x => x.Name)
                .ToList();

            if(memberRoles.Count == 0)
            {
                await SimpleReply("You don't have any roles for me to remove!", false, false);
                return;
            }

            var embed = BasicEmbed()
                .WithDescription("Select the number(s) that correspond to the role you wish to remove\n");

            for(int i = 0; i < memberRoles.Count; i++)
            {
                string name = memberRoles[i].Name;

                if(name.Length < 25)
                {
                    int amount = 25 - name.Length;
                    name = name.PadLeft(amount, ' ');
                }

                embed.AddField((i + 1).ToString(), name, true);
            }

            var response = await WithReply<string>(embed.Build(), true);

            string[] parameters = response.message.Content
                .Trim()
                .Replace(",", " ")
                .Split(" ");

            List<string> removed = new List<string>();
            await Context.TriggerTypingAsync();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    index -= 1;
                    if (index < 0 || index >= memberRoles.Count)
                        continue;

                    await Context.Member.RevokeRoleAsync(memberRoles[index]);
                    await Task.Delay(250);

                    removed.Add(memberRoles[index].Name);
                }
            }

            await Context.TriggerTypingAsync();
            await CleanupAsync();

            if (removed.Count > 0)
            {
                embed = BasicEmbed()
                    .WithDescription("The following roles were removed:");

                for (int i = 0; i < removed.Count; i++)
                    embed.AddField((i + 1).ToString(), removed[i], true);

                await SimpleReply(embed.Build(), false);
            }
            else
                await SimpleReply(BasicEmbed()
                    .WithDescription($"{Context.Member.Mention},\n No changes were made")
                    .Build(), false);
        }

        protected override string GetDefaultAuthorIcon() => "https://vignette.wikia.nocookie.net/harrypotter/images/6/62/Sorting_Hat.png/revision/latest?cb=20161120072849";
        protected override string GetDefaultAuthorName() => "Bouncer Hat";
        protected override string GetDefaultDescription() => "Select the corresponding number(s) to leave a class";
        protected override string GetDefaultHeadline() => "Leave Class(es)";
        protected override TimeSpan? GetDefaultTimeoutOverride() => null;

        protected override string GetDefaultExtensionData() => Newtonsoft.Json.JsonConvert.SerializeObject(
            new ExtensionConfig
            {
                BlacklistedRoles = new List<string>()
                {
                    "Moderator",
                    "Admin"
                }
            });
    }
}

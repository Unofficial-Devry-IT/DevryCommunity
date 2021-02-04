using DevryService.Bot.Exceptions;
using DevryServices.Common.Extensions;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryService.Bot.Wizards
{
    public class JoinRoleWizard : WizardBase
    {
        class ExtensionConfig
        {
            public List<string> BlacklistedRoles = new List<string>();
        }

        private ExtensionConfig _extensionConfig;

        public JoinRoleWizard(CommandContext context) : base(context)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            _extensionConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtensionConfig>(ExtensionData);
        }

        protected override async Task ExecuteAsync()
        {
            var lowercasedRoleNames = _extensionConfig.BlacklistedRoles.Select(x => x.ToLower());

            await Context.TriggerTypingAsync();
            var roles = Context.Guild.Roles
                .Where(x => !lowercasedRoleNames.Contains(x.Value.Name.ToLower()))
                .OrderBy(x => x.Value.Name)
                .Select(x => x.Value)
                .ToList();

            List<string> courseTypes = roles.Select(x => x.Name.Replace("-", " ").Split(" ").First())
                .Distinct()
                .ToList();

            var embed = BasicEmbed()
                .WithDescription("Which course(s) are you currently attending/teaching? Below is a list of categories. \nPlease type in the number(s) associated with the course\n");

            for (int i = 0; i < courseTypes.Count; i++)
                embed.AddField(i.ToString(), courseTypes[i], true);

            var primaryMessage = await WithReply<string>(embed.Build());

            if(primaryMessage.message.Content.IsNullOrEmpty())
            {
                await SimpleReply(BasicEmbed()
                    .WithDescription("Invalid input.")
                    .WithColor(DiscordColor.Red), false);
                throw new StopWizardException(AuthorName);
            }

            string[] parameters = primaryMessage.message.Content.Replace(",", " ").Split(" ");

            // Groups together course identifiers
            Dictionary<string, List<DiscordRole>> selectedGroups = new Dictionary<string, List<DiscordRole>>();

            // This maps the role to a number which the user will type in
            Dictionary<int, DiscordRole> roleMap = new Dictionary<int, DiscordRole>();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    if (index < 0 || index > courseTypes.Count)
                        Console.WriteLine($"Invalid Input");
                    else selectedGroups.Add(courseTypes[index], roles.Where(x => x.Name.ToLower().StartsWith(courseTypes[index].ToLower())).ToList());
                }
            }

            int current = 0;
            foreach(var key in selectedGroups.Keys)
            {
                embed = BasicEmbed().WithDescription($"Select the number associated with the class(es) you wish to join\n\n{key}:\n");

                foreach(var item in selectedGroups[key])
                {
                    embed.AddField((current + 1).ToString(), item.Name, true);
                    roleMap.Add(current, item);
                    current++;
                }

                await SimpleReply(embed.Build(), true);
            }

            var classResponse = await WithReply<string>(embed.Build());
            
            if(classResponse.message.Content.Trim().IsNullOrEmpty())
            {
                await SimpleReply(BasicEmbed().WithDescription("Invalid Input").WithColor(DiscordColor.Red).Build(), false);
                throw new StopWizardException(AuthorName);
            }

            parameters = classResponse.message.Content.Trim().Replace(",", " ").Split(" ");

            List<string> appliedRoles = new List<string>();
            await Context.TriggerTypingAsync();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    index -= 1;

                    if (index < 0 || index >= roleMap.Count)
                        Console.WriteLine($"Invalid input: {index + 1}");
                    else
                    {
                        await Context.Member.GrantRoleAsync(roleMap[index]);
                        await Task.Delay(500);
                        appliedRoles.Add(roleMap[index].Name);
                    }
                }
            }

            await Context.TriggerTypingAsync();
            await CleanupAsync();

            if (appliedRoles.Count > 0)
            {
                embed = BasicEmbed()
                    .WithDescription($"Hey, {Context.Member.DisplayName}, the following roles were applied");

                for (int i = 0; i < appliedRoles.Count; i++)
                    embed.AddField((i + 1).ToString(), appliedRoles[i]);

                await SimpleReply(embed.Build(), false);
            }
            else
                await SimpleReply(BasicEmbed()
                    .WithDescription($"Hey, {Context.Member.DisplayName}, no changes were applied")
                    .Build(), false);
        }

        protected override string GetDefaultAuthorIcon() => "https://vignette.wikia.nocookie.net/harrypotter/images/6/62/Sorting_Hat.png/revision/latest?cb=20161120072849";
        protected override string GetDefaultAuthorName() => "Sorting Hat";
        protected override string GetDefaultDescription() => "Allows a user to join their fellow classmates";
        protected override string GetDefaultHeadline() => "Lets get you settled";
        protected override TimeSpan? GetDefaultTimeoutOverride() => null;
        protected override string GetDefaultExtensionData() => Newtonsoft.Json.JsonConvert.SerializeObject(
            new ExtensionConfig
            {
                BlacklistedRoles = new List<string>()
               {
                   "Moderator",
                    "Admin",
                    "Hardware",
                    "Networking",
                    "Programmer",
                    "Professor",
                    "Database",
                    "Pollmaster",
                    "See-All-Channels",
                    "Motivator",
                    "Server Booster",
                    "Devry Test Bot",
                    "Devry-Challenge-Bot",
                    "Devry-Service-Bot",
                    "DeVry-SortingHat",
                    "announcement permissions",
                    "@everyone"
               }
            });
    }
}

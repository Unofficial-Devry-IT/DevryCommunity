using DevryServiceBot.Commands.General;
using DevryServiceBot.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevryServiceBot.Wizards
{
    [WizardInfo(Name ="Helper Hat",
                Description = "Guides you",
                IconUrl = "https://www.iconfinder.com/data/icons/millionaire-habits-filledoutline/64/HELP_OTHERS_SUCCEED-bussiness-people-finance-marketing-milionaire_habits-512.png")]
    public class HelpWizard : Wizard
    {
        string[] addDeleteReactions;

        public HelpWizard(CommandContext context) : base(context.User.Id, context.Channel) 
        { 
              addDeleteReactions = new string[]
                                {
                                    Emojis.CheckMark.Text,
                                    Emojis.XMark.Text
                                };
        }

        struct MenuInfo
        {
            public string Icon;
            public string CommandName;
            public string Description;
        }

        const string Headline = "Devry Help Wizard. Please react to the appropriate emoji and we'll get you started!\n\n";

        (Dictionary<string, List<MenuInfo>> reactionToTypes, List<string> menuEntries) SetupMenu()
        {            
            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(Wizard)) && x.Name != typeof(HelpWizard).Name && !x.IsAbstract)
                .OrderBy(x=>x.Name)
                .ToList();

            List<string> entries = new List<string>();
            Dictionary<string, List<MenuInfo>> collection = new Dictionary<string, List<MenuInfo>>();

            foreach(Type type in types)
            {
                WizardInfoAttribute info = type.GetCustomAttribute<WizardInfoAttribute>();                

                if (info == null)
                {
                    Console.WriteLine($"No info for: {type.Name}");
                    continue;
                }                

                // If wizard isn't meant to appear in help menu, skip it
                if (info.IgnoreHelpWizard)
                    continue;

                // Help wizard requires an emoji for reaction purposes
                if (string.IsNullOrEmpty(info.ReactionEmoji))
                    continue;


                // Must have a valid icon
                if (string.IsNullOrEmpty(info.CommandName))
                    continue;

                MenuInfo menuInfo = new MenuInfo
                {
                    CommandName = info.CommandName,
                    Icon = info.ReactionEmoji,
                    Description = info.Description
                };

                // Don't add duplicate emojis (each wizard should have it's own unless grouped)
                if (collection.ContainsKey(info.ReactionEmoji))
                {
                    collection[info.ReactionEmoji].Add(menuInfo);
                    continue;
                }

                if (!string.IsNullOrEmpty(info.GroupDescription) && !string.IsNullOrEmpty(info.Group))
                {
                    entries.Add($"{info.ReactionEmoji} - {info.GroupDescription}");
                    collection.Add(info.ReactionEmoji, new List<MenuInfo>() { menuInfo });
                    continue;
                }                

                entries.Add($"{info.ReactionEmoji} - {info.Description}");
                collection.Add(info.ReactionEmoji, new List<MenuInfo>() { menuInfo });
            }

            collection.Add(":email:", new List<MenuInfo>()
            {
                new MenuInfo
                {
                    CommandName = "HelpCommands.InivteLink",
                    Icon = ":email:",
                    Description = "Use this to obtain an invitation link! Send it to all who wish to join the army of knowledge seeking minions!"
                }
            });

            entries.Add($"{collection.Last().Key} - {collection.Last().Value.First().Description}");

            return (collection, entries);
        }

        bool IsNegative(MenuInfo info) => info.CommandName.ToLower().Contains("delete") || info.CommandName.ToLower().Contains("leave");

        public override async Task StartWizard(CommandContext context)
        {
            var wizardData = SetupMenu();

            const string basic = "Devry Help Wizard. Please react to the appropriate emoji and we'll get you started!\n\n";

            string mainMenu = basic + string.Join("\n\n", wizardData.menuEntries);

            var menuMessage = await WizardReply(context, mainMenu, true);

            foreach(var reactionName in wizardData.reactionToTypes.Keys)
            {
                await Task.Delay(500);
                await menuMessage.CreateReactionAsync(DiscordEmoji.FromName(Bot.Discord, reactionName));
            }

            var reaction = await GetUserReactionTo(menuMessage);

            // At this point we can remove the emojis/reactions
            await menuMessage.DeleteAllReactionsAsync();

            var submenu = wizardData.reactionToTypes[reaction.GetDiscordName()];
            MenuInfo reactionInfo = new MenuInfo();

            // So far there's no need for > 2
            if(submenu.Count == 2)
            {
                string submenuText = basic;
                
                MenuInfo positive = submenu.FirstOrDefault(x => !IsNegative(x));
                MenuInfo negative = submenu.FirstOrDefault(x => IsNegative(x));
                
                int pi = submenu.FindIndex((item) => item.CommandName == positive.CommandName);

                positive.Icon = Emojis.CheckMark.Text;
                negative.Icon = Emojis.XMark.Text;

                submenuText += $"{positive.Icon} - {positive.Description}\n\n";
                submenuText += $"{negative.Icon} - {negative.Description}\n\n";

                menuMessage = await WizardReplyEdit(menuMessage, submenuText, false);

                foreach(var subitem in new List<MenuInfo>() { positive, negative })
                {
                    await Task.Delay(500);
                    await menuMessage.CreateReactionAsync(DiscordEmoji.FromName(Bot.Discord, subitem.Icon));
                }

                reaction = await GetUserReactionTo(menuMessage);

                if (reaction.GetDiscordName().Equals(Emojis.CheckMark.Text, StringComparison.OrdinalIgnoreCase))
                    reactionInfo = positive;
                else
                    reactionInfo = negative;
            }
            else
            {
                reactionInfo = submenu.Where(x => x.Icon == reaction.GetDiscordName()).FirstOrDefault();
            }


            string typeName = reactionInfo.CommandName.Split(".").First();
            var commandType = Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(x => x.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase))
                                .FirstOrDefault();

            if(commandType == null)
            {
                Console.WriteLine($"Was unable to find command associated with: {reactionInfo.CommandName}");
                await Cleanup();
                return;
            }
            else
            {
                var instance = Activator.CreateInstance(commandType);
                var member = commandType.GetMethod(reactionInfo.CommandName.Split(".").Last());

                if(member == null)
                {
                    Console.WriteLine($"Was unable to find command associated with: {reactionInfo.CommandName}");
                    await Cleanup();
                    return;
                }

                // cleanup from this wizard
                await Cleanup();
                member.Invoke(instance, new object[] { context });
            }

        }
    }
}

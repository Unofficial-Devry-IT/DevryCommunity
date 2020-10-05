using DevryService.Commands;
using DevryService.Core;
using DevryService.Core.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using DescriptionAttribute = DevryService.Core.Util.DescriptionAttribute;

namespace DevryService.Wizards
{
    public struct MainPage
    {
        public Dictionary<string, SubPage> Wizards { get; set; }
        public SubPage Misc { get; set; }
    }

    public struct SubPage
    {
        public string Description { get; set; }
        public Dictionary<string, PageEntry> Options { get; set; }
    }

    public struct PageEntry
    {
        public string Description { get; set; }
        public Action<CommandContext> Command { get; set; }
    }

    [WizardInfo(Name = "Helper Hat",
                Description = "Gives guidance",
                IconUrl = "https://www.iconfinder.com/data/icons/millionaire-habits-filledoutline/64/HELP_OTHERS_SUCCEED-bussiness-people-finance-marketing-milionaire_habits-512.png")]
    public class HelpWizard : Wizard
    {
        const string Headline = "Devry Help Wizard. Please react to the appropriate emoji and we'll get you started!\n\n";
        const string POSITIVE_EMOJI = ":white_check_mark:";
        const string NEGATIVE_EMOJI = ":negative_squared_cross_mark:";

        MainPage MainMenu = new MainPage()
        {
            Misc = new SubPage()
            {
                Options = new Dictionary<string, PageEntry>()
            },
            Wizards = new Dictionary<string, SubPage>()
        };

        public HelpWizard(ulong userId, DiscordChannel channel) : base(userId, channel)
        {
        }

        bool HasAuth(DiscordMember member, RequireRolesAttributeAttribute attribute)
        {
            return member.Roles.Select(x => x.Name).Intersect(attribute.RoleNames).Count() > 0;
        }

        async Task GetIMiscCommands(CommandContext context)
        {
            foreach(var type in Assembly.GetExecutingAssembly().GetTypes().Where(x=>x.GetInterfaces().Contains(typeof(IMiscCommand))))
            {
                // Class level authorization
                if (type.GetCustomAttribute<RequireRolesAttributeAttribute>() != null)
                    if (!(await type.GetCustomAttribute<RequireRolesAttributeAttribute>().CanExecute(context, false)))
                        continue;

                foreach(var methodInfo in type.GetMethods().Where(x=>x.GetCustomAttribute<WizardCommandInfo>() != null))
                {
                    WizardCommandInfo info = methodInfo.GetCustomAttribute<WizardCommandInfo>();

                    // Shouldn't appear in the help wizard
                    if (info.IgnoreHelpWizard)
                        continue;

                    if (methodInfo.GetCustomAttribute<RequireRolesAttributeAttribute>() != null)
                        if (!(await methodInfo.GetCustomAttribute<RequireRolesAttributeAttribute>().CanExecute(context, false)))
                            continue;

                    PageEntry entry = new PageEntry()
                    {
                        Description = info.Description,
                        Command = (context) =>
                        {
                            var temp = Activator.CreateInstance(type);
                            methodInfo.Invoke(temp, new object[] { context });
                        }
                    };

                    MainMenu.Misc.Options.Add(info.Emoji, entry);
                }
            }
        }

        async Task GetIAddRemoveWizards(CommandContext context)
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IAddRemoveCommand))))
            {
                DescriptionAttribute classDescription = type.GetCustomAttribute<DescriptionAttribute>();

                SubPage page = new SubPage() { Options = new Dictionary<string, PageEntry>(), Description = classDescription.Text};

                // Grab the methods we need from IAddRemoveCommand
                MethodInfo createMethod = type.GetMethods().FirstOrDefault(x => x.Name.Equals("create", StringComparison.OrdinalIgnoreCase));
                MethodInfo deleteMethod = type.GetMethods().FirstOrDefault(x => x.Name.Equals("delete", StringComparison.OrdinalIgnoreCase));

                bool canAdd = true,
                     canDelete = true;

                // RequireRolesAttribute will be utilized on methods to restrict the users who can access things
                if (createMethod.GetCustomAttribute<RequireRolesAttributeAttribute>() != null)
                {
                    var requirements = createMethod.GetCustomAttribute<RequireRolesAttributeAttribute>();
                    if (!HasAuth(context.Member, requirements))
                        canAdd = false;
                }

                if (deleteMethod.GetCustomAttribute<RequireRolesAttributeAttribute>() != null)
                {
                    var requirements = deleteMethod.GetCustomAttribute<RequireRolesAttributeAttribute>();
                    if (!HasAuth(context.Member, requirements))
                        canDelete = false;
                }

                // Attributes which contain information about the method
                WizardCommandInfo positiveInfo = createMethod.GetCustomAttribute<WizardCommandInfo>();
                WizardCommandInfo negativeInfo = deleteMethod.GetCustomAttribute<WizardCommandInfo>();

                if (canAdd)
                    page.Options.Add(POSITIVE_EMOJI, new PageEntry
                    {
                        Description = positiveInfo.Description,
                        Command = (context) =>
                        {
                            var temp = Activator.CreateInstance(type);
                            createMethod.Invoke(temp, new object[] { context });
                        }
                    });

                if(canDelete)
                    page.Options.Add(NEGATIVE_EMOJI, new PageEntry
                    {
                        Description = negativeInfo.Description,
                        Command = (context) =>
                        {
                            var temp = Activator.CreateInstance(type);
                            deleteMethod.Invoke(temp, new object[] { context });
                        }
                    });

                // Only add to menu if there were options available to the current member in context
                if(page.Options.Count > 0)
                    MainMenu.Wizards.Add(classDescription.Icon, page);
            }
        }

        async Task<DiscordEmoji> MainWindow(CommandContext context)
        {
            
            var menuMessage = string.Join("\n\n", MainMenu.Wizards.Select(x => $"{x.Key} - {x.Value.Description}")) + "\n\n";
            menuMessage += string.Join("\n\n", MainMenu.Misc.Options.Select(x => $"{x.Key} - {x.Value.Description}"));

            if(menuMessage.Length == 0)
            {
                menuMessage = "It appears you don't have sufficient permissions. Please contact a moderator to get squared away!";
                this.WizardMessage = await WizardReply(context, menuMessage);
                return null;
            }
            else
                this.WizardMessage = await WizardReply(context, menuMessage);

            foreach (var reactionName in MainMenu.Wizards.Select(x=>x.Key))
            {
                await Task.Delay(500);
                await WizardMessage.CreateReactionAsync(DiscordEmoji.FromName(Bot.Discord, reactionName));
            }

            foreach(var reactionName in MainMenu.Misc.Options.Select(x=>x.Key))
            {
                await Task.Delay(500);
                await WizardMessage.CreateReactionAsync(DiscordEmoji.FromName(Bot.Discord, reactionName));
            }

            var reaction = await GetUserReactionTo(WizardMessage);
            await WizardMessage.DeleteAllReactionsAsync();

            return reaction;
        }

        async Task DisplaySubMenu(CommandContext context, DiscordEmoji emoji)
        {
            string name = emoji.GetDiscordName();
            Dictionary<string, PageEntry> optionsMenu;

            if (MainMenu.Wizards.Any(x => name.Equals(x.Key, StringComparison.OrdinalIgnoreCase)))
                optionsMenu = MainMenu.Wizards.FirstOrDefault(x => name.Equals(x.Key, StringComparison.OrdinalIgnoreCase)).Value.Options;
            else if (MainMenu.Misc.Options.ContainsKey(name))
            {
                await Cleanup();
                optionsMenu = MainMenu.Misc.Options;
                optionsMenu[name].Command(context);
                return;
            }
            else
                return;

            string options = string.Join("\n\n", optionsMenu.Select(x => $"{x.Key} - {x.Value.Description}"));
            WizardMessage = await WizardReplyEdit(WizardMessage, options, false);

            foreach(var icon in optionsMenu.Keys)
            {
                await Task.Delay(500);
                await WizardMessage.CreateReactionAsync(DiscordEmoji.FromName(Bot.Discord, icon));
            }

            var reaction = await GetUserReactionTo(WizardMessage);

            await Cleanup();

            if (optionsMenu.ContainsKey(reaction.GetDiscordName()))
            {
                var entry = optionsMenu[reaction.GetDiscordName()];
                entry.Command(context);
            }
            else
            {
                await context.RespondAsync(embed: new DiscordEmbedBuilder()
                    .WithAuthor(AuthorName, null, AuthorIcon)
                    .WithTitle("Error")
                    .WithDescription("An error occurred with this wizard")
                    .WithColor(DiscordColor.IndianRed)
                    .WithFooter("Badger will look into it")
                    .Build());
            }            
        }

        public override async Task StartWizard(CommandContext context)
        {
            await GetIAddRemoveWizards(context);
            await GetIMiscCommands(context);

            var reaction = await MainWindow(context);
            
            if (reaction == null)
            {
                await Cleanup();
                return;
            }

            await DisplaySubMenu(context, reaction);
        }
    }
}

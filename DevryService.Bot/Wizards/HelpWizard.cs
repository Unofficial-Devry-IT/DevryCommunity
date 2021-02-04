using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DevryService.Bot.Wizards
{
    public class HelpWizard : WizardBase
    {
        public HelpWizard(CommandContext context) : base(context)
        {
        }

        Dictionary<string, (Type type, MethodInfo method)> commandsToMethods = new Dictionary<string, (Type type, MethodInfo method)>();
        const string YES_EMOJI = ":white_check_mark:";
        const string NO_EMOJI = ":negative_squared_cross_mark:";

        protected override void Initialize()
        {
            base.Initialize();
            initializeCommandsToMethods();
        }

        void initializeCommandsToMethods()
        {
            Type[] types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => !x.IsAbstract && !x.IsInterface && x.Name.ToLower().EndsWith("command"))
                .ToArray();

            foreach(Type type in types)
            {
                MethodInfo info = type.GetMethods().FirstOrDefault(x => x.GetCustomAttribute<CommandAttribute>() != null);

                if (info != null)
                    commandsToMethods.Add(info.GetCustomAttribute<CommandAttribute>().Name, (type, info));
            }
        }

        protected override async Task ExecuteAsync()
        {
            var embed = BasicEmbed();
            List<string> currentEmojis = new List<string>();
            await Context.TriggerTypingAsync();

            // This is to create our first main window
            
        }

        protected override string GetDefaultAuthorIcon() => "https://www.iconfinder.com/data/icons/millionaire-habits-filledoutline/64/HELP_OTHERS_SUCCEED-bussiness-people-finance-marketing-milionaire_habits-512.png";
        protected override string GetDefaultAuthorName() => "Helper Hat";
        protected override string GetDefaultDescription() => "Gives guidance";
        protected override string GetDefaultHeadline() => "Devry Help Wizard. Please react to the appropriate emoji and we'll get you started!\n\n";
        protected override TimeSpan? GetDefaultTimeoutOverride() => null;
    }
}

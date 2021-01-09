using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Commands
{
    using Core;
    using DevryService.Wizards.Admin;
    using DSharpPlus.CommandsNext;
    using System.IO;
    using System.Reflection;
    using DevryService.Wizards;

    public static class CommandSettingsUtil
    {
        static string SettingsDir => Path.Join(Directory.GetCurrentDirectory(), "Configs");

        static Dictionary<string, Func<CommandConfig>> CommandMapper = new Dictionary<string, Func<CommandConfig>>()
        {
            { "viewWelcomeConfig", ViewWelcomeConfig },
            { "inviteLinkConfig", InviteLinkConfig },
            { "viewStatsConfig", ViewStatsConfig },
            { "helpCommandConfig", HelpCommandConfig },
            { "createClassConfig", ()=>{ return new CreateClassWizard(null).DefaultCommandConfig(); } },
            { "deleteEventConfig", ()=>{return new DeleteEventWizard(null).DefaultCommandConfig(); } },
            { "createEventConfig", ()=>{return new CreateEventWizard(null).DefaultCommandConfig(); } },
            { "joinCommandConfig", ()=>{return new JoinRoleWizard(null).DefaultCommandConfig(); } },
            { "leaveCommandConfig", ()=>{return new LeaveRoleWizard(null).DefaultCommandConfig(); } },
            { "codeCommandConfig", ()=>{return new SnippetWizard(null).DefaultCommandConfig(); } }
        };

        static void Print(string message, ConsoleColor color = ConsoleColor.White, bool newLine = true)
        {
            Console.ForegroundColor = color;

            if (newLine)
                Console.WriteLine(message);
            else
                Console.Write(message);

            Console.ResetColor();
        }

        public static string[] InitializeSettings()
        {
            List<string> configFiles = new List<string>();

            // We need to acquire ALL configuration types from the project
            Type[] wizardConfigTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => !x.IsAbstract && x.IsAssignableTo(typeof(WizardConfig)))
                .ToArray();

            // Let's print this out to console for... debugging purposes
            foreach(var type in wizardConfigTypes)
            {
                Print("Discovered config type", newLine: false);
                Print($" '{type.Name}'", ConsoleColor.Cyan);
            }

            // Let's acquire ALL wizards which utilize this config directly
            // Each wizard has the suffix 'wizard'
            Type[] wizards = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => !x.IsAbstract && x.Name.ToLower().EndsWith("wizard") && !wizardConfigTypes.Contains(x))
                .ToArray();

            Console.WriteLine();

            if (!Directory.Exists(SettingsDir))
                Directory.CreateDirectory(SettingsDir);

            foreach(var type in wizards)
            {
                Print("Discovered wizard ", newLine: false);
                Print($"'{type.Name}'", ConsoleColor.DarkYellow);

                // Try and match the config type with the wizard (without the word config the name should match..)
                Print($"\tInitializing config...", ConsoleColor.Yellow);
                Type configType = wizardConfigTypes.FirstOrDefault(x => type.Name.ToLower().Contains(x.Name.ToLower().Replace("config", "")));

                string configFilePath = Path.Combine(SettingsDir, $"{configType.Name}.json");
                configFiles.Add(configFilePath);

                if (File.Exists(configFilePath))
                {
                    Print($"Located config for ", newLine: false);
                    Print($"'{configType.Name}'", ConsoleColor.DarkYellow);
                    continue;
                }

                // No config... no work... continue onward
                if(configType == null)
                {
                    Print("\tCould not locate config for ", newLine: false);
                    Print($"'{type.Name}'", ConsoleColor.DarkRed);
                    continue;
                }

                Type instanceType = null;
                if (type.IsGenericType)
                    instanceType = type.MakeGenericType(configType);
                else
                    instanceType = type;

                // We need to retrieve the 'default' settings for this wizard since we established there was no 'Config' folder
                object wizardInstance = Activator.CreateInstance(instanceType, args: new object[] { null });

                // Need to acquire the 'DefaultSettings' method which will be converted into JSON
                MethodInfo info = instanceType.GetMethods().FirstOrDefault(x => x.Name.ToLower().Equals("defaultsettings"));
                object defaultSettings = info.Invoke(wizardInstance, null);

                // Convert our settings object into JSON string
                string settings = Newtonsoft.Json.JsonConvert.SerializeObject(defaultSettings);

                string json = "{" + $"\"{configType.Name}\":" + settings + "}";
                // Write to console for debugging purposes
                Console.WriteLine($"\n\n{json}\n\n");

                File.WriteAllText(configFilePath, json);
            }

            foreach(var pair in CommandMapper)
            {
                string filepath = Path.Combine(SettingsDir, $"{pair.Key}.json");
                configFiles.Add(filepath);

                if (File.Exists(filepath))
                {
                    Print($"Located command config --> ", newLine: false);
                    Print($"'{pair.Key}'", ConsoleColor.Cyan);
                    continue;
                }

                string settings = Newtonsoft.Json.JsonConvert.SerializeObject(pair.Value());
                string json = "{" + $"\"{pair.Key}\":" + settings + "}";

                Console.WriteLine($"\n\n{json}\n\n");
                File.WriteAllText(filepath, json);
            }

            return configFiles.ToArray();
        }

        public static T FindConfig<T>(string name) where T : CommandConfig
        {
            string path = Path.Join(SettingsDir, $"{name}.json");

            if(File.Exists(path))
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }

            return null;
        }

        public static MessageConfig ViewWelcomeConfig()
        {
            MessageConfig config = FindConfig<MessageConfig>("viewWelcomeConfig");

            string contents = $"Welcome, [USER], to the Unofficial DeVry IT Discord! A community-built by students to help foster education and professional growth; please " +
                $"introduce yourself in the #welcome-page channel and join our main discussions in the #main-hub-general. \n\n" +
                $"You can find your fellow classmates 1 of 3 ways. \n" +
                $"1.) Utilize the `!help` command, then react to the appropriate emoji\n" +
                $"2.) Utilize the `!join` command to bypass the `help` wizard.\n" +
                $"3.) Inform us which classes you're in, a @Moderator will gladly assist you.\n\n" +
                $"Thanks, and welcome to the Unofficial DeVry IT Discord!";

            if (config == null)
                return new MessageConfig
                {
                    Name = "View Welcome Message",
                    Description = "View the message that gets sent to newcomers when they join!",
                    Contents = contents
                };

            return config;
        }

        public static EmbedConfig InviteLinkConfig()
        {
            EmbedConfig config = FindConfig<EmbedConfig>("inviteLinkConfig");

            if (config == null)
                config = new EmbedConfig
                {
                    Name = "Recruiter Hat",
                    Title = "Invitation",
                    Emoji = ":email:",
                    Description = "Spread the word! Get your fellow classmates to join us!",
                    Contents = "Spread the word, our trusted scout! Spread the word " +
                    "of our kingdom! Amass an army of knowledge seeking minions! Lay waste " +
                    "to the legions of doubt and uncertainty!!",
                    Fields = new List<string>()
                    {
                        "Invite|https://discord.io/unofficial-DevryIT"
                    },
                    Footer = "Minions of knowledge! Assembblleeee!",
                    IgnoreHelpWizard = false
                };

            return config;
        }

        public static CommandConfig HelpCommandConfig()
        {
            CommandConfig config = FindConfig<CommandConfig>("helpCommandConfig");

            if (config == null)
                config = new CommandConfig
                {
                    Name = "Help",
                    Description = "A wizard shall appear and guide you along"
                };

            return config;
        }

        public static CommandConfig ViewStatsConfig()
        {
            CommandConfig config = FindConfig<CommandConfig>("viewStatsConfig");

            if (config == null)
                config = new CommandConfig
                {
                    Name = "View Stats",
                    Description = "Not fully implemented",
                    Icon = "https://alifeofproductivity.com/wp-content/uploads/2013/06/stat.001.jpg"
                };

            return config;
        }
    }
}

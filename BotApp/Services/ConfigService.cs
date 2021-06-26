using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotApp.Interaction;
using Domain.Entities;
using Domain.Entities.ConfigTypes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BotApp.Services
{
    public class ConfigService : IConfigService
    {
        /// <summary>
        /// Attempt to retrieve the proper display name for specified type
        /// </summary>
        /// <remarks>
        /// By default the display name will be the same as the given type name
        /// If a display attribute exists it will use the name property on it
        /// If a display name attribute exists it will use the DisplayName property on it
        /// </remarks>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetDisplayName(Type type)
        {
            string name = type.Name;

            var displayAttribute = type.GetCustomAttribute<DisplayAttribute>();
            
            if (displayAttribute != null && !string.IsNullOrEmpty(displayAttribute.Name))
                name = displayAttribute.Name;
            else
            {
                var displayName = type.GetCustomAttribute<DisplayNameAttribute>();

                if (displayName != null && !string.IsNullOrEmpty(displayName.DisplayName))
                    name = displayName.DisplayName;
            }
            
            return name;
        }
        
        public async Task InitializeInteractionConfigs()
        {
            // We need to get all interactions from the assembly
            var interactionTypes = Assembly.GetExecutingAssembly()
                                                    .ExportedTypes
                                                    .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(InteractionBase)))
                                                    .ToList();

            /*
             *  Default configurations shall be provided if they do not exist
             *  Will be up to the moderators / administrators to fine tune
             *  These configurations when the application is first launched
             */
            foreach (var type in interactionTypes)
            {
                // Don't need to initialize a config that already exists
                if (Bot.Instance.Context.Configs.Any(x =>
                    x.ConfigName.Equals(type.Name, StringComparison.CurrentCultureIgnoreCase)))
                    continue;
                
                InteractionConfig config = new InteractionConfig();
                
                config.AuthorName = type.Name;
                config.DisplayName = GetDisplayName(type);
                config.TimeoutOverride = TimeSpan.FromMinutes(5); // default timout of 5 minutes seems reasonable
                
                Config typeConfig = new Config();
                typeConfig.ConfigName = type.Name;
                typeConfig.ConfigType = ConfigType.INTERACTION;

                typeConfig.ExtensionData = Newtonsoft.Json.JsonConvert.SerializeObject(config);

                await Bot.Instance.Context.Configs.AddAsync(typeConfig);
            }

            await Bot.Instance.Context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task InitializeCommandConfigs()
        {
            var commandTypes = Assembly.GetExecutingAssembly()
                .ExportedTypes
                .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(BaseCommandModule)))
                .ToList();

                
            foreach (var type in commandTypes)
            {
                string prefix = type.Name.Replace("Command", "");

                var commandAttribute = type
                    .GetMethods()
                    .FirstOrDefault(x => x.GetCustomAttribute<CommandAttribute>() != null);

                // This attribute is required so we can properly link the discord command
                if (commandAttribute == null)
                    continue;
                
                // If a command with this typename exists OR an interaction that contains this type's prefix (remove the Command from name)
                if (Bot.Instance.Context.Configs.Any(x =>
                    x.ConfigName.Equals(type.Name, StringComparison.CurrentCultureIgnoreCase) ||
                    x.ConfigName.StartsWith(prefix))) 
                    continue;

                CommandConfig config = new CommandConfig();
                config.AuthorName = type.Name;
                config.DisplayName = GetDisplayName(type);
                config.TimeoutOverride = TimeSpan.FromMinutes(5);
                config.DiscordCommand = commandAttribute.Name;

                Config typeConfig = new Config();
                typeConfig.ConfigName = type.Name;
                typeConfig.ConfigType = ConfigType.COMMAND;
                typeConfig.ExtensionData = Newtonsoft.Json.JsonConvert.SerializeObject(config);

                await Bot.Instance.Context.Configs.AddAsync(typeConfig);
            }

            await Bot.Instance.Context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
using DevryService.Database.Contracts;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using DSharpPlus.CommandsNext.Attributes;

namespace DevryService.Bot.Commands
{
    public abstract class BaseCommand : BaseCommandModule, IDiscordCommand
    {
        protected virtual string GetCommandName()
        {
            var methods = GetType().GetMethods().Where(x => x.GetCustomAttribute<CommandAttribute>() != null);

            // Should technically have this attribute on the method in question, but in the event this changes later
            // we support taking the command type and parsing it into a command format
            // I.E    CreateEventCommand --> becomes --> create-event
            if (methods.Any())
                return methods.First().Name;
            else
            {
                string typename = GetType().Name.Replace("Command","").ToLower();
                string commandName = string.Empty;                

                for(int i = 0; i < typename.Length; i++)
                {
                    if(i == 0)
                    {
                        commandName += char.ToLower(typename[i]);
                        continue;
                    }

                    if(char.IsUpper(typename[i]))
                    {
                        commandName += $"-{char.ToLower(typename[i])}";
                    }
                }

                return commandName;
            }
        }

        protected abstract IList<string> GetRestrictedRoles();
        protected abstract string GetDescription();
        protected abstract string GetEmoji();
        protected abstract TimeSpan? GetTimeoutOverride();

        /// <summary>
        /// Command name that represents this command
        /// </summary>
        public string CommandName => GetCommandName();

        /// <summary>
        /// Default description of this command
        /// </summary>
        public string Description => GetDescription();

        /// <summary>
        /// Default emoji which represents this command
        /// </summary>
        public string Emoji => GetEmoji();

        /// <summary>
        /// Default Timeout Override
        /// </summary>
        public TimeSpan? TimeoutOverride => GetTimeoutOverride();

        /// <summary>
        /// Default roles that can run this command (empty == anyone)
        /// </summary>
        public IList<string> RestrictedRoles => GetRestrictedRoles();

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(CommandContext context);
    }
}

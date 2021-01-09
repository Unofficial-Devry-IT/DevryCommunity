using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DevryService.Core.Util;
using DSharpPlus.Entities;

namespace DevryService.Commands
{
    public class ViewCommandsCommand : BaseCommandModule, IDiscordCommand
    {
        /// <summary>
        /// Retrieve all commands within this assembly which utilize <see cref="IDiscordCommand"/>
        /// </summary>
        /// <param name="excludeAdmin">Should the algorithm include commands denoted with <see cref="RequireRolesAttributeAttribute"/>?</param>
        /// <returns>Dictionary of commands based on criteria. Key: <see cref="string"/> - command name. Value: <see cref="string"/> - description of command</returns>
        Dictionary<string, string> getCommandMatrix(bool excludeAdmin)
        {
            Dictionary<string, string> matrix = new Dictionary<string, string>();

            // Find all types within this assembly that follow the IDiscordCommand contract
            var commandTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetInterfaces()
                             .Contains(typeof(IDiscordCommand)));

            foreach(Type type in commandTypes)
            {
                MethodInfo[] methods = type.GetMethods();

                /*
                    Each command should at least have a 
                        CommandAttribute
                    Optionally
                        WizardCommandInfo
                        RequireRolesAttributeAttribute (I know it is repetitive but I did not create this one....)
                 */
                foreach (MethodInfo method in methods)
                {
                    CommandAttribute command = method.GetCustomAttribute<CommandAttribute>();
                    WizardCommandInfo info = method.GetCustomAttribute<WizardCommandInfo>();
                    RequireRolesAttribute require = method.GetCustomAttribute<RequireRolesAttribute>();

                    // If the method doesn't have a command attribute... well it isn't used for commands
                    if (command == null)
                        continue;

                    // If we are not including administrative stuff and we are requiring a role... just skip
                    if (excludeAdmin && require != null)
                        continue;

                    string name = command.Name;
                    string description = info != null ? info.Description : string.Empty;


                    matrix.Add(name, description);
                }
            }

            return matrix;
        }

        /// <summary>
        /// Displays commands using a <see cref="DiscordEmbedBuilder"/>
        /// </summary>
        /// <param name="context">Current context in which to send the embedded message</param>
        /// <param name="commands">Commands the user requested</param>
        async Task showCommands(CommandContext context, Dictionary<string, string> commands)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                .WithTitle("Available Commands")
                .WithDescription("You're currently viewing a list of generated commands")
                .WithColor(DiscordColor.Purple)
                .WithAuthor(name: "Knowledge Hat", iconUrl: null);

            foreach(var pair in commands)
                builder.AddField(pair.Key, pair.Value == string.Empty ? "No Description available" : pair.Value, true);

            await context.Channel.SendMessageAsync(embed: builder.Build());
        }

        /// <summary>
        /// View commands based on role
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [Command("view-commands")]
        public async Task ViewCommands(CommandContext context)
        {
            ulong userId = context.User.Id;

            var member = await context.Guild.GetMemberAsync(userId);

            bool excludeAdmin = true;

            // Member must be a moderator or admin in order to view all the commands
            if (member.Roles.Any(x => x.Name.ToLower() == "moderator" || x.Name.ToLower() == "admin"))
                excludeAdmin = false;

            var commands = getCommandMatrix(excludeAdmin);
            await showCommands(context, commands);
        }
    }
}

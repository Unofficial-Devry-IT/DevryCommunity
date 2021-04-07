using System.Threading.Tasks;
using BotApp.Interaction.Roles;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BotApp.Commands.Roles
{
    public class JoinRoleCommand : BaseCommand
    {
        [Command("join")]
        public override async Task ExecuteAsync(CommandContext context)
        {
            var interaction = new JoinRoleInteraction(context);
            await interaction.Run();
        }
    }
}
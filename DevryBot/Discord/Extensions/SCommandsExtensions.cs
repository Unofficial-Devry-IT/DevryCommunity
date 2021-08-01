using System.Linq;
using DisCatSharp.SlashCommands;

namespace DevryBot.Discord.Extensions
{
    public static class SCommandsExtensions
    {
        public static void RegisterCommandsFromAssembly<TMarker>(this SlashCommandsExtension slashCommandsExtension)
        {
            var commands = typeof(TMarker).Assembly.ExportedTypes
                                                    .Where(x => x.BaseType == typeof(SlashCommandModule))
                                                    .ToList();

            foreach (var command in commands)
                slashCommandsExtension.RegisterCommands(command, 618254766396538901);
        }
    }
}
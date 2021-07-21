using System.Threading.Tasks;
using DSharpPlusNextGen.SlashCommands;

namespace DevryBot.Discord.SlashCommands.Snippets
{
    public class ShowSnippet : SlashCommandModule
    {
        [SlashCommand("snippet", "View code samples from a variety of languages")]
        public static async Task Command(InteractionContext context)
        {
            
        }
    }
}
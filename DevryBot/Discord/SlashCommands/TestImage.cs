using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DevryBot.Services;
using DisCatSharp.Entities;
using DisCatSharp.SlashCommands;
using ImageCreator.Services;

namespace DevryBot.Discord.SlashCommands
{
    public class TestImage : SlashCommandModule
    {
        public IImageService ImageService { get; set; }
        public IGamificationService GameService { get; set; }
        public IBot Bot { get; set; }

        [SlashCommand("test", "Command that's under test")]
        public async Task Command(InteractionContext context)
        {
            if (!await context.ValidateGuild())
                return;

            await context.ImThinking();

            const ulong messageId = 874373221460348958;
            const ulong categoryId = 5;
            const string correctReaction = ":zero:";

            await GameService.ManualReward(messageId, "How can we add comments in PHP?", correctReaction, 5, categoryId);
            
            /*
             
             */

        }
    }
}
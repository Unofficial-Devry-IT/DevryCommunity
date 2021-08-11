using System;
using System.Threading.Tasks;
using DevryBot.Discord.SlashCommands.Filters;
using DevryBot.Services;
using DisCatSharp.SlashCommands;
using Microsoft.Extensions.Logging;

namespace DevryBot.Discord.SlashCommands.Gamification
{
    public class ManualReward : SlashCommandModule
    {
        public IGamificationService Game { get; set; }
        public ILogger<ManualReward> Logger { get; set; }

        [SlashCommand("manual-reward", "Reward users (mod only)")]
        [RequireModerator]
        public async Task Command(InteractionContext context,
            [Option("MessageId", "Message Id")] ulong messageId,
            [Option("Title","Question text")] string question,
            [Option("CorrectAnswer", "emoji")] string correctAnswer,
            [Option("Reward", "Score to add")] double reward,
            [Option("CategoryId", "number in database")]  ulong categoryId
        )
        {
            try
            {
                Logger.LogInformation(
                    $"Manually Rewarding for question {question} - for {reward} points. Correct answer should have been {correctAnswer}");
                await Game.ManualReward(messageId, question, correctAnswer, reward, categoryId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
        }
    }
}
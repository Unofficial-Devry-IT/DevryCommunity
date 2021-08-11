using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DevryDomain.Models;
using UnofficialDevryIT.Architecture.Models;

namespace DevryBot.Services
{
    public interface IGamificationService
    {
        /// <summary>
        /// Create a new challenge for our service
        /// </summary>
        /// <param name="challenge"></param>
        /// <param name="category"></param>
        /// <param name="responses"></param>
        /// <returns></returns>
        Task<ResultObject> CreateChallenge(Challenge challenge, string category, List<ChallengeResponse> responses);

        /// <summary>
        /// Manually reward the users who reacted appropriately
        /// </summary>
        /// <param name="discordMessageId">ID of discord message</param>
        /// <param name="title">Ideally the question that was originally asked</param>
        /// <param name="correctReactionWas">Discord version of emoji</param>
        /// <param name="rewardAmount">Amount to reward users</param>
        /// <param name="categoryId">Category this falls under (2 by default)</param>
        /// <returns></returns>
        /// <remarks>
        ///  Discord reactions are surrounded by colons. Our challenges have a number between 0-9
        /// :zero:
        /// :one:
        /// :two:
        /// etc...
        /// </remarks>
        Task ManualReward(ulong discordMessageId, string title, string correctReactionWas, double rewardAmount,
            ulong categoryId = 2);
        
        /// <summary>
        /// Delete a challenge given its primary key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteChallenge(ulong id);
        
        /// <summary>
        /// Get all the challenges associated with this service
        /// </summary>
        /// <returns></returns>
        Task<List<Challenge>> GetChallenges();
        
        /// <summary>
        /// Get the currently active daily challenge
        /// </summary>
        /// <returns></returns>
        Task<Challenge> GetDailyChallenge();
        
        /// <summary>
        /// Post the next daily challenge (if applicable)
        /// </summary>
        /// <returns></returns>
        Task<ResultObject> PostChallenge();

        /// <summary>
        /// Process the current daily challenge (if applicable)
        /// </summary>
        /// <returns></returns>
        Task ProcessChallenge();
    }
}
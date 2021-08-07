using System.Collections.Generic;
using System.Threading.Tasks;
using DevryDomain.Models;

namespace ChallengeAssistant.Interfaces
{
    /// <summary>
    /// Wrapper for aggregating challenge questions
    /// from various websites that offer it
    /// </summary>
    public interface IChallengeApi
    {
        /// <summary>
        /// Name of the service
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Site that is being utilized for retrieving questions
        /// </summary>
        string Site { get; }
        
        /// <summary>
        /// Retrieves and parses response from <see cref="Site"/>'s API
        /// </summary>
        /// <returns></returns>
        Task<List<Challenge>> RetrieveQuestionsAsync();
    }
}
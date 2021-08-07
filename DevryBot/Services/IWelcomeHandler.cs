using System;
using System.Threading.Tasks;
using DisCatSharp.Entities;

namespace DevryBot.Services
{
    /// <summary>
    /// Handles the welcoming of users to the community
    /// </summary>
    public interface IWelcomeHandler
    {
        /// <summary>
        /// Basedo n interaction ID format -- adds that associated role to <paramref name="member"/>
        /// </summary>
        /// <param name="member"></param>
        /// <param name="interactionId"></param>
        Task AddRoleToMember(DiscordMember member, string interactionId);
        
        /// <summary>
        /// Adds a class that we're expecting users to be coming from (DeVry)
        /// Along with an expiration time for when the class button should
        /// no longer be appended to the welcome message
        /// </summary>
        /// <param name="role"></param>
        /// <param name="expirationTime"></param>
        void AddClass(DiscordRole role, DateTime expirationTime);
        
        /// <summary>
        /// Adds member to greeting queue
        /// </summary>
        /// <param name="member"></param>
        void AddMember(DiscordMember member);
    }
}
using System.Threading.Tasks;
using DevryBot.Discord.Attributes;
using DevryBot.Services;
using DisCatSharp.Entities;

namespace DevryBot.Discord.Interactions
{
    [InteractionName(InteractionConstants.NEW_CHALLENGE)]
    [InteractionName(InteractionConstants.END_CHALLENGE)]
    public class ChallengeActionInteractionHandler : IInteractionHandler
    {
        private readonly IGamificationService _service;
        
        public ChallengeActionInteractionHandler(IGamificationService service)
        {
            _service = service;
        }
        
        public async Task Handle(DiscordMember member, DiscordInteraction interaction, string[] values, string interactionId)
        {
            switch (interactionId)
            {
                case InteractionConstants.NEW_CHALLENGE:
                    await _service.PostChallenge();
                    break;
                case InteractionConstants.END_CHALLENGE:
                    await _service.ProcessChallenge();
                    break;
            }
        }
    }
}
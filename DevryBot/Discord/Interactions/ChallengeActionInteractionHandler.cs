using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Attributes;
using DevryBot.Services;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using Microsoft.Extensions.Logging;

namespace DevryBot.Discord.Interactions
{
    [InteractionName(InteractionConstants.NEW_CHALLENGE)]
    [InteractionName(InteractionConstants.END_CHALLENGE)]
    public class ChallengeActionInteractionHandler : IInteractionHandler
    {
        private readonly IGamificationService _service;
        private readonly ILogger<ChallengeActionInteractionHandler> _logger;
        
        public ChallengeActionInteractionHandler(IGamificationService service, ILogger<ChallengeActionInteractionHandler> logger)
        {
            _service = service;
            _logger = logger;
        }
        
        public async Task Handle(DiscordMember member, ComponentInteractionCreateEventArgs args)
        {
            switch (args.Id)
            {
                case InteractionConstants.NEW_CHALLENGE:
                    await _service.PostChallenge();
                    break;
                case InteractionConstants.END_CHALLENGE:
                    await _service.ProcessChallenge();
                    break;
                default:
                    _logger.LogError($"Couldn't find a method to handle {args.Id}");
                    break;
            }
        }
    }
}
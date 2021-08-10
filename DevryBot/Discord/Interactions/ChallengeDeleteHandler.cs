using System.Linq;
using System.Threading.Tasks;
using DevryBot.Discord.Attributes;
using DevryBot.Services;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;

namespace DevryBot.Discord.Interactions
{
    [InteractionName(InteractionConstants.CHALLENGE_DELETE)]
    public class ChallengeDeleteHandler : IInteractionHandler
    {
        private readonly IGamificationService _service;
        
        public ChallengeDeleteHandler(IGamificationService service)
        {
            _service = service;
        }
        
        public async Task Handle(DiscordMember member, ComponentInteractionCreateEventArgs args)
        {
            if(ulong.TryParse(args.Id.Split("_").First(), out ulong id))
            {
                await _service.DeleteChallenge(id);
                if (args.Message != null)
                    await args.Message.DeleteAsync();
            }
        }
    }
}
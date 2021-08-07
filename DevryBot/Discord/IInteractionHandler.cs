using System.Threading.Tasks;
using DisCatSharp.Entities;

namespace DevryBot.Discord
{
    public interface IInteractionHandler
    {
        Task Handle(DiscordMember member, DiscordInteraction interaction, string[] values, string interactionId);
    }
}
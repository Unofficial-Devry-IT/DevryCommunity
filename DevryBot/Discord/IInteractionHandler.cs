using System.Threading.Tasks;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;

namespace DevryBot.Discord
{
    public interface IInteractionHandler
    {
        Task Handle(DiscordMember member, ComponentInteractionCreateEventArgs args);
    }
}
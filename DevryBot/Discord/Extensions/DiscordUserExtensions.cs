using System.Linq;
using System.Threading.Tasks;
using DSharpPlusNextGen.Entities;

namespace DevryBot.Discord.Extensions
{
    public static class DiscordUserExtensions
    {
        public static async Task<bool> IsModerator(this DiscordUser user)
        {
            var member = await Bot.Instance.MainGuild
                                           .GetMemberAsync(user.Id);

            return member
                .Roles
                .Any(x => x.Name.ToLower().Contains("moderator"));
        }
    }
}
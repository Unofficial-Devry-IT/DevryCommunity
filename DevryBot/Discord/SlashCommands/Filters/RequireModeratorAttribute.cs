using System.Threading.Tasks;
using DevryBot.Discord.Extensions;
using DisCatSharp.SlashCommands;

namespace DevryBot.Discord.SlashCommands.Filters
{
    /// <summary>
    /// Checks to make sure a user has a moderator role
    /// </summary>
    public class RequireModeratorAttribute : SlashCheckBaseAttribute
    {
        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            return await ctx.User.IsModerator();
        }
    }
}
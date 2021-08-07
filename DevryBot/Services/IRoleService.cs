using System.Collections.Generic;
using DisCatSharp.Entities;

namespace DevryBot.Services
{
    public interface IRoleService
    {
        IReadOnlyDictionary<ulong, DiscordRole> GetBlacklistedRolesDict(ulong guildId);
    }
}
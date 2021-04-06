using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using BotApp;
using DSharpPlus.Entities;

namespace BotApp.Services
{
    public class DiscordRoleService
    {
        private readonly ICurrentUserService _userService;

        private readonly string MODERATOR_ROLE = "Moderator";
        private readonly string[] ADMIN_ROLES = {"admin", "moderator"};
        
        public DiscordRoleService(ICurrentUserService userService)
        {
            _userService = userService;
        }

        public async Task<bool> HasRole(string roleName)
        {
            DiscordMember member = await Bot.Instance.MainGuild.GetMemberAsync(ulong.Parse(_userService.UserId));

            if (member == null)
                return false;

            return member.Roles.Any(x => x.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<bool> IsModerator()
        {
            DiscordMember member = await Bot.Instance.MainGuild.GetMemberAsync(ulong.Parse(_userService.UserId));

            if (member == null)
                return false;

            return member.Roles.Any(x => x.Name.Equals(MODERATOR_ROLE, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<bool> IsAdmin()
        {
            DiscordMember member = await Bot.Instance.MainGuild.GetMemberAsync(ulong.Parse(_userService.UserId));

            if (member == null)
                return false;

            return member.Roles.Any(x => ADMIN_ROLES.Contains(x.Name.ToLower()));
        }
        
        
    }
}
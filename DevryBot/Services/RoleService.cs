using System;
using System.Collections.Generic;
using System.Linq;
using DisCatSharp.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UnofficialDevryIT.Architecture.Extensions;

namespace DevryBot.Services
{
    public class RoleService : IRoleService
    {
        private readonly IConfiguration _configuration;
        private readonly IBot _bot;
        private readonly ILogger<RoleService> _logger;

        private Dictionary<ulong, List<DiscordRole>> _blacklistedRolesByGuild = new();
        
        public RoleService(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<RoleService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _bot = serviceProvider.GetRequiredService<IBot>();
        }
        
        void InitializeBlacklistedRolesByName(ulong? guildId = null)
        {
            if (guildId.HasValue)
            {
                DiscordGuild guild = _bot.Client.Guilds[guildId.Value];
                
                if(!_blacklistedRolesByGuild.ContainsKey(guild.Id))
                    _blacklistedRolesByGuild.Add(guild.Id, new());
                else
                    _blacklistedRolesByGuild[guild.Id].Clear();
                
                foreach (string roleName in _configuration.GetEnumerable("Discord:BlacklistedRoles"))
                {
                    var role = guild.Roles.FirstOrDefault(x =>
                        x.Value.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));

                    if (role.Key > 0)
                        _blacklistedRolesByGuild[guild.Id].Add(role.Value);
                    else
                        _logger.LogWarning($"Could not locate {roleName} within guild {guild.Name}");
                }
                
                return;
            }
            
            _blacklistedRolesByGuild.Clear();
            
            foreach (DiscordGuild guild in _bot.Client.Guilds.Values)
            {
                _blacklistedRolesByGuild.Add(guild.Id, new List<DiscordRole>());
                
                foreach (string roleName in _configuration.GetEnumerable("Discord:BlacklistedRoles"))
                {
                    var role = guild.Roles.FirstOrDefault(x =>
                        x.Value.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));

                    if (role.Key > 0)
                        _blacklistedRolesByGuild[guild.Id].Add(role.Value);
                    else
                        _logger.LogWarning($"Could not locate role {roleName} within guild {guild.Name}");
                }
            }
            
        }
        
        public IReadOnlyDictionary<ulong, DiscordRole> GetBlacklistedRolesDict(ulong guildId)
        {
            Dictionary<ulong, DiscordRole> results = new();

            if (!_bot.Client.Guilds.ContainsKey(guildId))
                return results;

            if (_blacklistedRolesByGuild.ContainsKey(guildId))
                return _blacklistedRolesByGuild[guildId]
                    .ToDictionary(x=>x.Id, x=>x);
            
            InitializeBlacklistedRolesByName(guildId);
            
            if (_blacklistedRolesByGuild.ContainsKey(guildId))
                return _blacklistedRolesByGuild[guildId]
                    .ToDictionary(x=>x.Id, x=>x);
            
            return results;
        }
    }
}
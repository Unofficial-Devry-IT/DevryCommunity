using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlusNextGen;
using DSharpPlusNextGen.Entities;

namespace DevryBot.Discord.Extensions
{
    public static class RolesDictionaryExtensions
    {
        /// <summary>
        /// Does the <paramref name="searchName"/> match any of the 
        /// naming conventions for our role?
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="searchName"></param>
        /// <param name="startsWith">Dictates the type of search to be performed</param>
        /// <returns>True if a match was found, otherwise false</returns>
        public static bool MatchesRoleConvention(this string roleName, string searchName, bool startsWith = false)
        {
            if (startsWith)
                return roleName.ToLower().StartsWith(searchName.ToLower());
            
            return roleName.Equals(searchName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Converts a string into the Devry Discord Role naming convention
        /// if it isn't already
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>Name that matches the expected naming convention</returns>
        public static string ToDiscordRuleNameConvention(this string roleName)
        {
            if (roleName.Contains(" "))
                return roleName;

            for (int i = 0; i < roleName.Length; i++)
                if (char.IsDigit(roleName, i))
                    return roleName.Substring(0, i) + " " + roleName.Substring(i);

            return roleName;
        }

        /// <summary>
        /// Remove blacklisted roles from <paramref name="roles"/>
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="blacklistedRoles"></param>
        /// <returns>Listed without blacklisted roles</returns>
        public static List<DiscordRole> RemoveBlacklistedRoles(this IEnumerable<DiscordRole> roles,
            IEnumerable<ulong> blacklistedRoles)
        {
            var list = blacklistedRoles.ToList();
            
            return roles
                .Where(x => x != null && !list.Contains(x.Id) && !x.Name.StartsWith("^"))
                .ToList();
        }

        public static List<DiscordRole> FindRolesWithName(this IEnumerable<DiscordRole> roles, string name,
            bool startsWith = false)
        {
            if (name.Length < 3)
                return new();

            /*
                Ensure our name is up to snuff
                Since this method ALWAYS starts with startsWith to false
                We can save time complexity by only updating the name
                the first pass through, otherwise it's a waste of resources / time
             */
            if (!startsWith)
                name = name.ToDiscordRuleNameConvention();

            // First lets find out if our search parameter is an exact match for an existing role
            var role = roles.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (role != null)
                return new() {role};

            var results = roles.Where(x => x.Name.MatchesRoleConvention(name, startsWith))
                .ToList();

            if (results.Count > 0)
                return results;
            
            return FindRolesWithName(roles, name.Substring(0, startsWith ? (name.Length - 1) : name.Length), true);
        }

        /// <summary>
        /// Attempt to locate roles that match, or start with
        /// <paramref name="name"/>
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="name"></param>
        /// <param name="startsWith"></param>
        /// <returns>List of <see cref="DiscordRole"/> that meet our criteria</returns>
        public static List<DiscordRole> FindRolesWithName(this IReadOnlyDictionary<ulong, DiscordRole> roles, string name, bool startsWith = false)
        {
            if (name.Length < 3)
                return new();

            /*
                Ensure our name is up to snuff
                Since this method ALWAYS starts with startsWith to false
                We can save time complexity by only updating the name
                the first pass through, otherwise it's a waste of resources/time
             */
            if(!startsWith)
                name = name.ToDiscordRuleNameConvention();


            /*
                First lets find out if our search parameter is an exact match for an existing
                role
             */

            var rolePair = roles.FirstOrDefault(x => x.Value.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if(rolePair.Key > 0)
                return new() { rolePair.Value };
            
            // Okay, do any of the roles at least START with our search parameter?
            var results = roles.Where(x => x.Value.Name.MatchesRoleConvention(name, startsWith))
                .Select(x=>x.Value)
                .ToList();

            if (results.Count > 0)
                return results;

            // Recursively go back until we get some results! -- or until we meet our exit case
            return FindRolesWithName(roles, name.Substring(0, startsWith ? (name.Length - 1) : name.Length), true);
        }
    }
}

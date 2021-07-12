using System.Collections.Generic;
using System.Linq;
using DevryCore.Extensions;
using DSharpPlusNextGen.Entities;
using Microsoft.Extensions.Configuration;

namespace DevryBot
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Retrieve blacklisted roles from configuration
        /// </summary>
        /// <param name="config"></param>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static List<ulong> BlacklistedRoles(this IConfiguration config, DiscordGuild guild)
        {
            var names = config.GetEnumerable("Discord:BlacklistedRoles").ToList();

            return guild
                .Roles
                .Where(x => names.Contains(x.Value.Name))
                .Select(x => x.Key)
                .ToList();
        }

        /// <summary>
        /// Retrieve list of majors that are available to the 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<string> Majors(this IConfiguration config)
        {
            return config.GetEnumerable("Discord:Majors").ToList();
        }

        public static string UhOhImage(this IConfiguration config) => config.GetValue<string>("Discord:UhOhImage");

        /// <summary>
        /// Retrieve the image used for tasks that are in queue to be processed
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string QueueImage(this IConfiguration config) => config.GetValue<string>("Discord:QueueImage");

        /// <summary>
        /// Retrieve the image used for completed tasks
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string CompletedImage(this IConfiguration config) =>
            config.GetValue<string>("Discord:CompletedImage");

        /// <summary>
        /// Retrieve the image used for warning embeds
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string WarningImage(this IConfiguration config) =>
            config.GetValue<string>("Discord:WarningImage");

        /// <summary>
        /// Retrieve the image used for error embeds
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string ErrorImage(this IConfiguration config) => config.GetValue<string>("Discord:ErrorImage");

        /// <summary>
        /// Retrieve the image used for invitation messages
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string InviteImage(this IConfiguration config) =>
            config.GetValue<string>("Discord:InviteImage");


        /// <summary>
        /// Retrieve the search criteria for determining if a link is a "discord invite"
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetInviteLinkSearchCriteria(this IConfiguration config) =>
            config.GetValue<string>("Discord:InviteLinkSearchCriteria");
    }
}
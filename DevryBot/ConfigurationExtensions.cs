using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevryCore.Extensions;
using DevryDomain.Models;
using DSharpPlusNextGen.Entities;
using Microsoft.Extensions.Configuration;

namespace DevryBot
{
    public static class ConfigurationExtensions
    {
        public static int DeleteReportAfterDuration(this IConfiguration config)
            => config.GetValue<int>("Discord:DeleteReportAfterDuration");
        
        /// <summary>
        /// Retrieves the storage location of where snippets are hiding
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string SnippetStorageLocation(this IConfiguration config) =>
            Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Data", config.GetValue<string>("Snippets:Storage"));

        /// <summary>
        /// Retrieves the colors to associate each language as
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Dictionary<string, LanguageInfo> SnippetLanguageColors(this IConfiguration config)
            => config.GetValue<Dictionary<string, LanguageInfo>>("Snippets:Languages");

        /// <summary>
        /// Retrieves the dictionary map of what file extension goes with what title
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static Dictionary<string, string> SnippetFileExtensions(this IConfiguration config)
            => config.GetValue<Dictionary<string, string>>("Snippets:FileExtensions");
        
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
        /// Retrieves the value for how long the welcome handler will keep track of the class(es) going on
        /// </summary>
        /// <param name="config"></param>
        /// <returns>Integer value - in hours - for duration length</returns>
        public static int InviteWelcomeDuration(this IConfiguration config) =>  
            config.GetValue<int>("WelcomeSettings:InviteWelcomeDuration");
        
        /// <summary>
        /// Retrieves the value for how long the interval is between welcoming people. Allows
        /// multiple people to be cached into the same message rather than multiple
        /// </summary>
        /// <param name="config"></param>
        /// <returns>Integer value in seconds</returns>
        public static int WelcomeMessageInterval(this IConfiguration config) =>
            config.GetValue<int>("WelcomeSettings:WelcomeMessageInterval");

        /// <summary>
        /// Retrieves the welcome message that will be displayed to users
        /// </summary>
        /// <param name="config"></param>
        /// <returns>Welcome message format</returns>
        public static string WelcomeMessage(this IConfiguration config) =>
            config.GetValue<string>("WelcomeSettings:WelcomeMessage");

        /// <summary>
        /// Retrieves the text that shall be used for invite embedded messages
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string InviteMessage(this IConfiguration config) =>
            config.GetValue<string>("Discord:InviteMessage");

        /// <summary>
        /// Retrieves the invitation link that shall be used for invites
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string InviteLink(this IConfiguration config) => config.GetValue<string>("Discord:InviteLink");

        /// <summary>
        /// Retrieves the footer text that shall be used for invite embedded messages
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string InviteFooter(this IConfiguration config) =>
            config.GetValue<string>("Discord:InviteFooter");
        
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
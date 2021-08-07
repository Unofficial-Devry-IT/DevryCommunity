using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevryDomain.Models;
using DisCatSharp.Entities;
using Microsoft.Extensions.Configuration;
using UnofficialDevryIT.Architecture.Extensions;

namespace DevryBot
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the base URL for the Devry Website
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string DevryWebsite(this IConfiguration config)
            => config.GetValue<string>("Discord:DevryWebsite");

        /// <summary>
        /// Get the reports URL based on <see cref="DevryWebsite"/>
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string DevryWebsiteReports(this IConfiguration config)
            => Path.Join(config.DevryWebsite(), "reports");
        
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
        /// Retrieve the search criteria for determining if a link is a "discord invite"
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetInviteLinkSearchCriteria(this IConfiguration config) =>
            config.GetValue<string>("Discord:InviteLinkSearchCriteria");
    }
}
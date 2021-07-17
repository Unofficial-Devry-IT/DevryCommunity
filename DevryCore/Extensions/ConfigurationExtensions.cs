using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DevryCore.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Retrieves an array portion from the configuration file
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="valuePath">Path to value (Account:Options:Roles)</param>
        /// <returns></returns>
        public static IEnumerable<string> GetEnumerable(this IConfiguration configuration, string valuePath)
        {
            return configuration.GetSection(valuePath).Get<string[]>();
        }

        /// <summary>
        /// Retrieves an array of a specified type from the configuration file
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="valuePath">Path to value (Account:Options:Roles)</param>
        /// <typeparam name="T">Type of value the array contains. Should be numeric in nature (ulong, double, etc)</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumerable<T>(this IConfiguration configuration, string valuePath) 
            where T : IComparable, IConvertible, IFormattable, IComparable<T>, IEquatable<T>
        {
            return configuration.GetSection(valuePath).Get<T[]>();
        }
    }
}
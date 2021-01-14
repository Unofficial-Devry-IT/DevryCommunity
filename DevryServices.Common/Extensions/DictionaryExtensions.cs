using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryServices.Common.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// This method is used to try to get a value in a dictionary if it exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            if(dictionary.TryGetValue(key, out var valueObj) && valueObj is T variable)
            {
                value = variable;
                return true;
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if not found
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var obj) ? obj : default(TValue);
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns a default value if can't be found
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            if (dictionary.TryGetValue(key, out var obj))
                return obj;

            return dictionary[key] = factory(key);
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns a default value if it cannot be found
        /// </summary>
        /// <typeparam name="TKey">Type of key used in dictionary</typeparam>
        /// <typeparam name="TValue">Type of value used in dictionary</typeparam>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value of</param>
        /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
        /// <returns>Value if found, default if it can't be found</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
        {
            return dictionary.GetOrAdd(key, k => factory());
        }
    }
}

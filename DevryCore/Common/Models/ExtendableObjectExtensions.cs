using System;
using System.Collections.Generic;
using DevryCore.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevryCore.Common.Models
{
    /// <summary>
    /// Adds functionality for entities of <see cref="IExtendableObject"/>
    /// </summary>
    public static class ExtendableObjectExtensions
    {
        /// <summary>
        /// Retrieve <paramref name="name"/> (key) from JSON structure
        /// </summary>
        /// <param name="extendableObject">Entity which contains data</param>
        /// <param name="name">Key to get data from</param>
        /// <param name="handleType"></param>
        /// <typeparam name="T">The type associated to the value at (key)<paramref name="name"/></typeparam>
        /// <returns>Value located at <paramref name="name"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T GetData<T>(this IExtendableObject extendableObject, string name, bool handleType = false)
        {
            CheckNotNull(extendableObject, name);

            if (extendableObject == null)
                throw new ArgumentNullException(nameof(extendableObject));
            
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        
            return extendableObject.GetData<T>(
                name,
                handleType
                    ? new JsonSerializer { TypeNameHandling = TypeNameHandling.All }
                    : JsonSerializer.CreateDefault()
            );
        }
        
        /// <summary>
        /// Attempts to retrieve value located at (key) <paramref name="name"/>
        /// </summary>
        /// <param name="extendableObject">Entity which contains the data</param>
        /// <param name="name">key from JSON strucutre to get data from</param>
        /// <param name="jsonSerializer"></param>
        /// <typeparam name="T">The type associated to the data attempting to be fetched</typeparam>
        /// <returns><typeparamref name="T"/> data</returns>
        public static T GetData<T>(this IExtendableObject extendableObject, string name, JsonSerializer jsonSerializer)
        {
            CheckNotNull(extendableObject, name);

            if (extendableObject.ExtensionData == null)
                return default(T);

            var json = JObject.Parse(extendableObject.ExtensionData);

            var prop = json[name];

            if (prop == null)
                return default(T);

            if (TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(T)))
                return prop.Value<T>();
            else
                return (T)prop.ToObject(typeof(T), jsonSerializer ?? JsonSerializer.CreateDefault());
        }

        /// <summary>
        /// Attempt to set the value of <paramref name="name"/>(key) in JSON structure to
        /// <paramref name="value"/>
        /// </summary>
        /// <param name="extendableObject">Entity containing data</param>
        /// <param name="name">key value in JSON structure</param>
        /// <param name="value">value that shall be set to <paramref name="name"/> key</param>
        /// <param name="handleType"></param>
        /// <typeparam name="T">Type of data that is being set</typeparam>
        public static void SetData<T>(this IExtendableObject extendableObject, string name, T value, bool handleType = false)
        {
            extendableObject.SetData(
               name,
               value,
               handleType
                   ? new JsonSerializer { TypeNameHandling = TypeNameHandling.All }
                   : JsonSerializer.CreateDefault());
        }

        /// <summary>
        /// Attempt to set value of <paramref name="name"/>(key) in JSON structure to <paramref name="value"/>
        /// </summary>
        /// <param name="extendableObject">Entity containing data</param>
        /// <param name="name">key in JSON structure which <paramref name="value"/> will be set to</param>
        /// <param name="value">Data which will be inserted into <paramref name="name"/> key</param>
        /// <param name="jsonSerializer"></param>
        /// <typeparam name="T">Type of data that shall get set</typeparam>
        public static void SetData<T>(this IExtendableObject extendableObject, string name, T value, JsonSerializer jsonSerializer)
        {
            CheckNotNull(extendableObject, name);

            if (jsonSerializer == null)
                jsonSerializer = JsonSerializer.CreateDefault();

            if(extendableObject.ExtensionData == null)
            {
                if (EqualityComparer<T>.Default.Equals(value, default(T)))
                    return;

                extendableObject.ExtensionData = "{}";
            }

            var json = JObject.Parse(extendableObject.ExtensionData);

            if (value == null || EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                if (json[name] != null)
                    json.Remove(name);
            }
            else if (TypeHelper.IsPrimitiveExtendedIncludingNullable(value.GetType()))
                json[name] = new JValue(value);
            else
                json[name] = JToken.FromObject(value, jsonSerializer);

            var data = json.ToString(Formatting.None);

            if (data == "{}")
                data = null;

            extendableObject.ExtensionData = data;
        }

        /// <summary>
        /// Delete/Remove entry from JSON structure
        /// </summary>
        /// <param name="extendableObject">Entity containing data</param>
        /// <param name="name">Delete the key associated with this name</param>
        /// <returns></returns>
        public static bool RemoveData(this IExtendableObject extendableObject, string name)
        {
            CheckNotNull(extendableObject, name);

            if (extendableObject.ExtensionData == null)
                return false;

            var json = JObject.Parse(extendableObject.ExtensionData);

            var token = json[name];
            if (token == null)
                return false;

            json.Remove(name);

            var data = json.ToString(Formatting.None);
            
            if (data == "{}")
                data = null;

            extendableObject.ExtensionData = data;

            return true;
        }

        /// <summary>
        /// Ensure all values within <paramref name="values"/> are not null
        /// </summary>
        /// <param name="values"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void CheckNotNull(params object[] values)
        {
            foreach (var value in values)
                if (value == null)
                    throw new ArgumentNullException();
        }
    }
}
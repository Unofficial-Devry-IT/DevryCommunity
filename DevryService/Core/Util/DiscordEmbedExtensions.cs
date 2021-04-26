using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus.Entities;

namespace DevryService.Core.Util
{
    public static class DiscordEmbedExtensions
    {
        /// <summary>
        /// Format a collection of items to display in a textual menu
        ///
        /// Formatted using YAML (index is the key)
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="collection">Items to show in menu. Utilizes the ToString method on each instance</param>
        /// <param name="zeroBased">Should the menu start at 0, or at 1</param>
        /// <typeparam name="T">Type of objects that are being utilized</typeparam>
        /// <returns><see cref="DiscordEmbedBuilder"/> with desired text</returns>
        public static DiscordEmbedBuilder FormatAsMenu<T>(this DiscordEmbedBuilder builder, IEnumerable<T> collection, bool zeroBased = false)
        {
            StringBuilder textBuilder = new StringBuilder();
            textBuilder.Append("\n```yaml\n");
            
            // avoid possible multiple enumerations over collection by converting to array
            var enumerable = collection as T[] ?? collection.ToArray();
            
            for(int i = 0; i < enumerable.Length; i++)
                if (enumerable[i] != null && !string.IsNullOrEmpty(enumerable[i].ToString()))
                    textBuilder.Append($"{(zeroBased ? i : i + 1)}: {enumerable[i]}\n");
            
            textBuilder.Append("```\n");

            builder.Description += textBuilder.ToString();
            return builder;
        }

        /// <summary>
        /// Alternative to <seealso cref="FormatAsMenu{T}(DSharpPlus.Entities.DiscordEmbedBuilder,System.Collections.Generic.IEnumerable{T},bool)"/>
        /// Allows you to dictate exactly how items appear in the menu
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="collection">Items to showcase in textual menu</param>
        /// <param name="toString">Function which will output the desired text</param>
        /// <param name="zeroBased">Should the menu start at 0, or 1</param>
        /// <typeparam name="T">Type of objects that are being shown in menu</typeparam>
        /// <returns><see cref="DiscordEmbedBuilder"/> with desired description</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DiscordEmbedBuilder FormatAsMenu<T>(this DiscordEmbedBuilder builder, IEnumerable<T> collection, Func<T, string> toString, bool zeroBased = false)
        {
            if (toString == null)
                throw new ArgumentNullException(nameof(toString),
                    "This function is required to properly showcase the objects in textual format");
            
            StringBuilder textBuilder = new StringBuilder();
            textBuilder.Append("\n```yaml\n");
            
            // avoid possible multiple enumerations over collection by converting to array
            var enumerable = collection as T[] ?? collection.ToArray();
            
            for(int i = 0; i < enumerable.Length; i++)
                if (enumerable[i] != null && !string.IsNullOrEmpty(toString(enumerable[i])))
                    textBuilder.Append($"{(zeroBased ? i : i + 1)}: {enumerable[i].ToString()}\n");
            
            textBuilder.Append("```\n");

            builder.Description += textBuilder.ToString();
            return builder;
        }

        /// <summary>
        /// Useful for our dictionaries where want to keep track of items between pages
        /// Like the other FormatAsMenu extensions this just formats a collection to be viewable via text
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="collection">Items to show in text format</param>
        /// <param name="mapper">Dictionary that's used to map our values as we go</param>
        /// <param name="toString">Function which will generate the text to display for each instance</param>
        /// <param name="startIndex">Number at which to start at when this gets displayed. Again for multipages</param>
        /// <param name="menuCount">Outputs the amount of items that got added to menu (cannot be null, cannot be empty string)</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DiscordEmbedBuilder FormatAsMenu<T>(this DiscordEmbedBuilder builder, IEnumerable<T> collection,
            ref Dictionary<int, T> mapper,
            Func<T, string> toString, int startIndex, out int menuCount)
        {
            if (toString == null)
                throw new ArgumentNullException(nameof(toString),
                    "This function is required to properly showcase the objects in textual format");
            
            StringBuilder textBuilder = new StringBuilder();
            textBuilder.Append("\n```yaml\n");
            
            // avoid possible multiple enumerations over collection by converting to array
            var enumerable = collection as T[] ?? collection.ToArray();

            // We only increment this number if the item is valid
            int count = 0;
            
            for(int i = 0; i < enumerable.Length; i++)
                if (enumerable[i] != null && !string.IsNullOrEmpty(toString(enumerable[i])))
                {
                    textBuilder.Append($"{i+startIndex}: {toString(enumerable[i])}\n");
                    mapper.Add(i, enumerable[i]);
                    count++;
                }
            
            textBuilder.Append("```\n");

            builder.Description += textBuilder.ToString();
            menuCount = count;
            return builder;
        }
    }
}
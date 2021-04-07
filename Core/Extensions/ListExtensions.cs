using System.Collections;
using System.Collections.Generic;

namespace Core.Extensions
{
    public static class ListExtensions
    {
        public static IDictionary<string, string> ToOptions(this IList<string> collection, bool zeroBased = true)
        {
            IDictionary<string, string> results = new Dictionary<string, string>();

            for (int i = 0; i < collection.Count; i++)
                results.Add($"{(zeroBased ? i : i+1)}", collection[i]);

            return results;
        }

        public static IDictionary<string, string> ToOptions(this IList<string> collection, int startingIndex = 0)
        {
            IDictionary<string, string> results = new Dictionary<string, string>();

            for (int i = startingIndex; i < collection.Count; i++)
                results.Add(i.ToString(), collection[i]);

            return results;
        }
    }
}
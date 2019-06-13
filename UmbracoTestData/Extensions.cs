using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UmbracoTestData
{
    internal static class Extensions
    {
        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        public static IEnumerable<IList<T>> Chunk<T>(this IEnumerable<T> source, uint chunkSize)
        {
            IList<T> clone = source.ToList();
            while (clone.Any())
            {
                yield return clone.Take((int)chunkSize).ToList();
                clone = clone.Skip((int)chunkSize).ToList();
            }
        }
        
        public static string ToCamelCase(this string str)
        {
            str = new CultureInfo("en-US", false).TextInfo.ToTitleCase(str);
            str = str.Replace(" ", "");
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
    }
}
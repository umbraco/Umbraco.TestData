using System;
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
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, uint chunkSize)
        {
            while (source.Any())
            {
                yield return source.Take((int)chunkSize);
                source = source.Skip((int)chunkSize);
            }
        }
        
        public static string ToCamelCase(this string str)
        {
            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
            str = cultInfo.ToTitleCase(str);
            str = str.Replace(" ", "");
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
    }
}
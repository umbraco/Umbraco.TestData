using System.Collections.Generic;
using System.Linq;

namespace UmbracoTestData
{
    public static class Extensions
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
    }
}
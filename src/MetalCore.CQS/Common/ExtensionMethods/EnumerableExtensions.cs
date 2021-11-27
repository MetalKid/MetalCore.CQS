using System.Collections.Generic;
using System.Linq;

namespace MetalCore.CQS.ExtensionMethods
{
    /// <summary>
    /// This class holds extension methods to add to the System.IEnumerable interface.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns a group of items based upon the given size until all are processed.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="size">The maximum number of items to return in a list.</param>
        /// <returns>A subset of the main list.</returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(
            this IEnumerable<T> source,
            int size)
        {
            if (source == null)
            {
                yield break;
            }

            T[] bucket = null;
            int count = 0;

            foreach (T item in source)
            {
                if (bucket == null)
                {
                    bucket = new T[size];
                }
                bucket[count++] = item;

                if (count != size)
                {
                    continue;
                }

                yield return bucket;

                bucket = null;
                count = 0;
            }

            // Return the last bucket with all remaining elements
            if (bucket != null && count > 0)
            {
                yield return bucket.Take(count);
            }
        }
    }
}
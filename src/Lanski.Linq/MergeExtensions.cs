using System.Collections.Generic;

namespace Lanski.Linq
{
    public static class MergeExtensions
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T item)
        {
            foreach (var e in enumerable)
            {
                yield return e;
            }

            yield return item;
        }
        
        public static IEnumerable<T> Concat<T>(this T item, IEnumerable<T> enumerable)
        {
            yield return item;

            foreach (var e in enumerable)
            {
                yield return e;
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Lanski.Linq
{
    public static class EnumerableOfNullableExtensions
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
            where T: struct
        {
            return enumerable.Where(x => x.HasValue).Select(x => x.Value);
        }
    }
}
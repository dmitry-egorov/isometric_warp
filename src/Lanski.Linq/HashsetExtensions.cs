using System.Collections.Generic;
using System.Linq;

namespace Lanski.Linq
{
    public static class HashsetExtensions
    {
        public static int SequenceHashCode<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return 0;
            
            return enumerable.Aggregate(0, (a, x) => (a * 397) ^ x.GetHashCode());
        }
    }
}
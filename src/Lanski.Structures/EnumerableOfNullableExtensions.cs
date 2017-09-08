using System;
using System.Collections.Generic;
using System.Linq;

namespace Lanski.Structures
{
    public static class EnumerableOfNullableExtensions
    {
        public static IEnumerable<T> SkipEmpty<T>(this IEnumerable<Possible<T>> enumerable) where T : class => 
            enumerable
                .Where(x => x.has_a_Value())
                .Select(x => x.must_have_a_Value())
        ;
        

        public static IEnumerable<T> SkipNull<T>(this IEnumerable<T?> enumerable) where T: struct => 
            enumerable.Where(x => x.HasValue).Select(x => x.Value)
        ;
    }
}
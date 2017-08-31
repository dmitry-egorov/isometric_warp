using System;
using System.Collections.Generic;
using System.Linq;

namespace Lanski.Structures
{
    public static class EnumerableOfNullableExtensions
    {
        public static IEnumerable<T> SkipEmpty<T>(this IEnumerable<Slot<T>> enumerable) where T : class => 
            enumerable
                .Where(x => x.Has_a_Value())
                .Select(x => x.Must_Have_a_Value().Otherwise(new InvalidOperationException("Unreachable. Value was checked")))
        ;
        

        public static IEnumerable<T> SkipNull<T>(this IEnumerable<T?> enumerable) where T: struct => 
            enumerable.Where(x => x.HasValue).Select(x => x.Value)
        ;
    }
}
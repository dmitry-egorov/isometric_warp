using System;
using System.Collections.Generic;
using Lanski.Structures;

namespace Lanski.SwiftLinq
{
    public static class LinqExtensions
    {
        public static Possible<T> SLast<T>(this IReadOnlyList<T> list) => list.Count == 0 ? Possible.Empty<T>() : list[list.Count - 1];
        
        public static Possible<T> SFirst<T>(this IReadOnlyList<T> list, Func<T, bool> condition)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (condition(item))
                    return item;
            }

            return Possible.Empty<T>();
        }
        
        public static bool SAny<T>(this IReadOnlyList<T> list, Func<T, bool> condition)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (condition(list[i]))
                    return true;
            }

            return false;
        }
    }
}
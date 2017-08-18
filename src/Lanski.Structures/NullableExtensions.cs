using System;

namespace Lanski.Structures
{
    public static class NullableExtensions
    {
        public static bool Is<T>(this T? nullable, Func<T, bool> selector)
            where T : struct
        {
            return nullable.Select(selector).GetValueOrDefault();
        }
        public static TResult? Select<T, TResult>(this T? nullable, Func<T, TResult> selector)
            where T : struct
            where TResult: struct
        {
            return !nullable.HasValue ? default(TResult?) : selector(nullable.Value);
        }        
    }
}
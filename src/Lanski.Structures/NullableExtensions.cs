using System;

namespace Lanski.Structures
{
    public static class NullableExtensions
    {
        public static TResult? Select<T, TResult>(this T? nullable, Func<T, TResult> selector)
            where T : struct
            where TResult: struct
        {
            return !nullable.HasValue ? default(TResult?) : selector(nullable.Value);
        }        
    }
}
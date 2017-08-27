using System;

namespace Lanski.Structures
{
    public static class NullableExtensions
    {
        public static void Do<T>(this T nullable, Action<T> action)
            where T: class
        {
            if (nullable != null)
                action(nullable);
        }
        
        public static bool Is<T>(this T nullable, Func<T, bool> selector)
            where T : class
        {
            return nullable.SelectValue(selector).GetValueOrDefault(false);
        }
        
        public static TResult? SelectValue<T, TResult>(this T nullable, Func<T, TResult> selector)
            where T : class 
            where TResult: struct 
        {
            return nullable != null ? selector(nullable) : default(TResult?);
        }
        
        public static void Do<T>(this T? nullable, Action<T> action)
            where T : struct
        {
            if (nullable.HasValue)
                action(nullable.Value);
        }
        
        public static bool Is<T>(this T? nullable, Func<T, bool> selector)
            where T : struct
        {
            return nullable.Select(selector).GetValueOrDefault();
        }
        
        public static TResult? Select<T, TResult>(this T? nullable, Func<T, TResult> selector)
            where T : struct
            where TResult: struct
        {
            return nullable.HasValue ? selector(nullable.Value) : default(TResult?);
        }

        public static T GetValueOr<T>(this T? nullable, Func<T> defaultFactory)
            where T : struct
        {
            return nullable ?? defaultFactory();
        }
    }
}
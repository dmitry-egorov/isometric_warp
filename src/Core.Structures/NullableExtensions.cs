namespace Core.Structures
{
    using System;

    public struct Optional<T>
    {
    }

    public static class NullableExtensions
    {
        public static TOut? Select<TIn, TOut>(this TIn? option, Func<TIn, TOut> selector)
            where TIn : struct
            where TOut : struct
        {
            return option.HasValue 
                ? (TOut?) selector(option.Value) 
                : null;
        }

        public static T? Do<T>(this T? option, Action<T> action)
            where T: struct
        {
            if (option.HasValue)
            {
                action(option.Value);
            }

            return option;
        }
    }
}
using System;
using JetBrains.Annotations;

namespace Lanski.Structures
{
    public static class NullableEx
    {
        public static T? From<T>(T value) where T: struct => value;
    }


    public static class NullableExtensions
    {
        public static void Do<T>(this T nullable, Action<T> action)
            where T: class
        {
            if (nullable != null)
                action(nullable);
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

        public static Slot<TResult> SelectManyRef<T, TResult>(this T? nullable, Func<T, Slot<TResult>> selector)
            where T : struct
            where TResult : class
        {
            return nullable.HasValue ? selector(nullable.Value) : default(Slot<TResult>);
        }
        
        public static Slot<TResult> SelectRef<T, TResult>(this T? nullable, Func<T, TResult> selector)
            where T : struct
            where TResult: class 
        => 
            nullable.HasValue ? selector(nullable.Value).As_a_Slot() : default(Slot<TResult>)
        ;

        public static TResult? Select<T, TResult>(this T? nullable, Func<T, TResult> selector)
            where T : struct
            where TResult: struct
        {
            return nullable.HasValue ? selector(nullable.Value) : default(TResult?);
        }

        public static T Value_Or<T>(this T? nullable, Func<T> defaultFactory)
            where T : struct => nullable ?? defaultFactory();

        public static bool doesnt_have_a_value<T>(this T? nullable) where T: struct => !nullable.Has_a_Value();
        public static bool Has_a_Value<T>(this T? nullable) where T: struct => nullable != null;
        public static bool doesnt_contain_a<T>(this T? nullable, out T v) where T : struct => !nullable.Has_a_Value(out v);
        
        public static bool Has_a_Value<T>(this T? nullable, out T o) where T: struct
        {
            var result = nullable.HasValue;
            o = result ? nullable.Value : default(T);
            return result;
        }
        
        public static T Must_Have_a_Value<T>(this T? nullable) where T: struct => 
            nullable.Has_a_Value(out var value) 
                ? value 
                : throw new InvalidOperationException()
        ;
    }
}
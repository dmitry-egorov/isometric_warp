using System;

namespace Lanski.Structures
{
    public static class Option
    {
        public static Option<T> Some<T>(T value)
        {
            return new Option<T>(value);
        }

        public static Option<T> None<T>()
        {
            return null;
        }

        public static bool HasValue<T>(this Option<T> option)
        {
            return option != null;
        }

        public static T GetOrDefault<T>(this Option<T> option)
        {
            return option.HasValue() ? option.Value : default(T);
        }

        public static T GetOr<T>(this Option<T> option, Func<T> defaultValue)
        {
            return option.HasValue() ? option.Value : defaultValue();
        }

        public static T GetOr<T>(this Option<T> option, T defaultValue)
        {
            return option.HasValue() ? option.Value : defaultValue;
        }

        public static bool DoesntHaveValue<T>(this Option<T> option)
        {
            return option == null;
        }

        public static Option<TTo> Select<TFrom, TTo>(this Option<TFrom> option, Func<TFrom, TTo> selector)
        {
            return option.HasValue() ? Some(selector(option.Value)) : None<TTo>();
        }

        public static void Do<T>(this Option<T> option, Action<T> action)
        {
            if (option.HasValue())
            {
                action(option.Value);
            }
        }

        public static Option<TTo> SelectMany<TFrom, TTo>(this Option<TFrom> option, Func<TFrom, Option<TTo>> selector)
        {
            return option.HasValue() ? selector(option.Value) : None<TTo>();
        }
    }

    public class Option<T>
    {
        public T Value { get; }

        public Option(T value)
        {
            Value = value;
        }
    }
}
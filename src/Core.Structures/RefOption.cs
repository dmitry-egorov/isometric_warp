namespace Core.Structures
{
    using System;

    public static class RefOption
    {

        public static RefOption<T> From<T>(T value) where T : class
        {
            return new RefOption<T>(value);
        }

        public static RefOption<T> Some<T>(T value) where T : class
        {
            return new RefOption<T>(value);
        }

        public static RefOption<T> None<T>() where T : class
        {
            return default(RefOption<T>);
        }
    }

    public struct RefOption<T>
        where T : class
    {
        public T Value { get; }
        public bool HasValue => !ReferenceEquals(Value, null);
        public bool DoesntHaveValue => ReferenceEquals(Value, null);

        public RefOption(T value)
        {
            Value = value;
        }

        public T GetOrNull()
        {
            return HasValue ? Value : null;
        }

        public RefOption<TTo> Select<TTo>(Func<T, TTo> selector) where TTo : class
        {
            return HasValue ? RefOption.Some(selector(Value)) : RefOption.None<TTo>();
        }

        public RefOption<TTo> SelectMany<TTo>(Func<T, RefOption<TTo>> selector) where TTo : class
        {
            return HasValue ? selector(Value) : RefOption.None<TTo>();
        }

        public void Do(Action<T> action)
        {
            if (HasValue)
            {
                action(Value);
            }
        }
    }
}
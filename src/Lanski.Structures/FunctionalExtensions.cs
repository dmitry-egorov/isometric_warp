using System;

namespace Lanski.Structures
{
    public static class FunctionalExtensions
    {
        public static Curry<T> Curry<T>(this Action<T> action, T value) => new Curry<T>(action, value);
    }

    public struct Curry<T>
    {
        private readonly Action<T> _action;
        private readonly T _value;

        public Curry(Action<T> action, T value)
        {
            _action = action;
            _value = value;
        }

        public void Invoke() => _action(_value);
    }
}
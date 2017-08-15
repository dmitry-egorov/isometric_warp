using System;

namespace Core.Structures
{
    public static class Lazy
    {
        public static Lazy<T> From<T>(Func<T> creator)
        {
            return new Lazy<T>(creator);
        }
    }

    public class Lazy<T>
    {
        private readonly Func<T> _creator;

        private bool _startedInitializing;
        private bool _initialized;
        private T _value;

        public T Value
        {
            get
            {
                if (_initialized)
                    return _value;

                if (_startedInitializing)
                    throw new InvalidOperationException("Recursive initialization");

                _startedInitializing = true;
                var value = _creator();

                _initialized = true;
                return _value = value;
            }
        }

        public Lazy(Func<T> creator)
        {
            _creator = creator;
        }

        public void Initiate()
        {
            var t = Value;
        }
    }
}
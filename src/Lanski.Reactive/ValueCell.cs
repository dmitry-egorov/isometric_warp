using System;

namespace Lanski.Reactive
{
    public class ValueCell<T> : ICell<T>, IConsumer<T>
        where T : struct
    {
        private readonly Stream<T> _stream = new Stream<T>();
        private T _lastValue;

        public ValueCell(T initialValue)
        {
            _lastValue = initialValue;
        }

        public T Value
        {
            get => _lastValue;
            set
            {
                if (_lastValue.Equals(value))
                    return;

                _lastValue = value;
                _stream.Next(value);
            }
        }

        public Action Subscribe(Action<T> action)
        {
            action(_lastValue);
            return _stream.Subscribe(action);
        }

        void IConsumer<T>.Next(T value)
        {
            Value = value;
        }
    }
}
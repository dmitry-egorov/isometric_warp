using System;

namespace Lanski.Reactive
{
    public class NullableCell<T> : ICell<T?>, IConsumer<T?>
        where T : struct
    {
        private readonly Stream<T?> _stream = new Stream<T?>();
        private T? _lastValue;

        public NullableCell(T? initialValue)
        {
            _lastValue = initialValue;
        }

        public T? s_Value
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

        public Action Subscribe(Action<T?> action)
        {
            action(_lastValue);
            return _stream.Subscribe(action);
        }

        void IConsumer<T?>.Next(T? value)
        {
            s_Value = value;
        }
    }
}
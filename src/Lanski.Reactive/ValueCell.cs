using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class ValueCellExtensions
    {
        public static ValueCell<Slot<T>> Empty<T>() => new ValueCell<Slot<T>>(Slot.Empty<T>());
        public static ValueCell<T> Create<T>(T initial) where T : struct => new ValueCell<T>(initial);
    }
    
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
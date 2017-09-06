using System;
using System.Collections.Generic;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public static class Cell
    {
        public static Cell<Possible<T>> Empty<T>() => new Cell<Possible<T>>(Possible.Empty<T>());
    }
    
    public class Cell<T> : ICell<T>, IConsumer<T>
    {
        private readonly Stream<T> _stream = new Stream<T>();
        private T _lastValue;

        public Cell(T initialValue)
        {
            _lastValue = initialValue;
        }

        public T s_Value
        {
            get => _lastValue;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_lastValue, value))
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
            s_Value = value;
        }
    }
}
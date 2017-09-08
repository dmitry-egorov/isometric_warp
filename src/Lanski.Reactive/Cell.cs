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
        private readonly EqualityComparer<T> the_equality_comparer;
        private T _lastValue;

        public Cell(T initialValue)
        {
            _lastValue = initialValue;
            the_equality_comparer = EqualityComparer<T>.Default;
        }

        public T s_Value
        {
            get => _lastValue;
            set
            {
                if (the_equality_comparer.Equals(_lastValue, value))
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
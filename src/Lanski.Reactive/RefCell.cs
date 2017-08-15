using System;

namespace Lanski.Reactive
{
    public class RefCell<T> : ICell<T>, IConsumer<T>
        where T: class 
    {
        private readonly Stream<T> _stream = new Stream<T>();
        private T _lastValue;

        public RefCell(T initialValue)
        {
            _lastValue = initialValue;
        }

        public T Value
        {
            get { return _lastValue; }
            set
            {
                if (_lastValue == null && value == null)
                    return;
                if (_lastValue != null && _lastValue.Equals(value))
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
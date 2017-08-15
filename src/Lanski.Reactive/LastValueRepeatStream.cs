using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public class LastValueRepeatStream<T> : IStream<T>, IConsumer<T>
    {
        private readonly Stream<T> _stream = new Stream<T>();
        private Option<T> _lastValue;

        public Action Subscribe(Action<T> action)
        {
            _lastValue.Do(action);
            return _stream.Subscribe(action);
        }

        public void Next(T value)
        {
            _lastValue = Option.Some(value);
            _stream.Next(value);
        }
    }
}
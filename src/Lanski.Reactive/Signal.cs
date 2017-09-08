using System;
using Lanski.Structures;

namespace Lanski.Reactive
{
    public class Signal<T> : IStream<T>, IConsumer<T>
    {
        private readonly Stream<T> _stream = new Stream<T>();
        private Possible<T> _last_value;

        public Action Subscribe(Action<T> action)
        {
            if (!_last_value.has_a_Value(out var value)) 
                return _stream.Subscribe(action);
            
            action(value);
            return () => { };
        }

        public void Next(T value)
        {
            _last_value = value;
            _stream.Next(value);
            _stream.UnsubscribeAll();
        }
    }
}
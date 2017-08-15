using System;
using System.Collections.Generic;

namespace Lanski.Reactive
{
    public class RepeatAllStream<T> : IStream<T>, IConsumer<T>
    {
        private readonly Stream<T> _stream = new Stream<T>();
        private readonly List<T> _values = new List<T>();
        
        public Action Subscribe(Action<T> action)
        {
            foreach (var value in _values)
            {
                action(value);
            }
            
            return _stream.Subscribe(action);
        }

        public void Next(T value)
        {
            _values.Add(value);
            _stream.Next(value);
        }
    }
}
using System;

namespace Lanski.Reactive
{
    public class EmptyStream<T> : IStream<T>
    {
        public Action Subscribe(Action<T> action) => () => { };
    }
}
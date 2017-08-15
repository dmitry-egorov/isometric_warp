using System;

namespace Lanski.Reactive
{
    public interface IStream<T>
    {
        Action Subscribe(Action<T> action);
    }
}
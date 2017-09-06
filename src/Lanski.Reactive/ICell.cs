namespace Lanski.Reactive
{
    public interface ICell<T> : IStream<T>
    {
        T s_Value { get; }
    }
}
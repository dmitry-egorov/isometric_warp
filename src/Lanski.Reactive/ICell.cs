namespace Lanski.Reactive
{
    public interface ICell<T> : IStream<T>
    {
        T Value { get; }
    }
}
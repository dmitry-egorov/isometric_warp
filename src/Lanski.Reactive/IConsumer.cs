namespace Lanski.Reactive
{
    public interface IConsumer<in T>
    {
        void Next(T value);
    }
}
namespace WarpSpace.Game.Tasks
{
    public interface ITask
    {
        void Performs_a_Step();
        bool is_Complete { get; }
        bool is_a<T>(out T the_variant);
    }
}
namespace WarpSpace.Game.Tasks
{
    public interface ITaskVariant<in TExecutor>
    {
        bool Performs_a_Step_On(TExecutor the_executor);
    }
}
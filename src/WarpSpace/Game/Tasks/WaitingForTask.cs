namespace WarpSpace.Game.Tasks
{
    public class WaitingForTask<T>: ITaskVariant<T>
    {
        public WaitingForTask(ITask the_task) => its_target_task = the_task;

        public bool Performs_a_Step_On(T _) => its_target_task.is_Complete;
        public override string ToString() => $"{nameof(T)} for {its_target_task}";

        private readonly ITask its_target_task;
    }
}
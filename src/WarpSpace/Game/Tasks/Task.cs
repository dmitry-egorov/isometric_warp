using Lanski.Structures;
using WarpSpace.Game.Battle.Tile;

namespace WarpSpace.Game.Tasks
{
    public class Task<TExecutor>: ITask
    {
        internal Task(ITaskVariant<TExecutor> the_variant, TExecutor the_executor, WAgenda the_agenda, Possible<WTasksHolder> the_possible_tasks_holder)
        {
            its_variant = the_variant;
            this.the_agenda = the_agenda;
            this.the_possible_tasks_holder = the_possible_tasks_holder;
            this.the_executor = the_executor;
        }

        public bool is_Complete => it_is_complete;
        public bool is_a<T>(out T the_variant)
        {
            if (its_variant is T the_actial_variant)
            {
                the_variant = the_actial_variant;
                return true;
            }

            the_variant = default(T);
            return false;
        }

        public ITaskVariant<TExecutor> s_Variant => its_variant;

        public void Performs_a_Step()
        {
            if (it_is_complete)
                return;
            
            it_is_complete = its_variant.Performs_a_Step_On(the_executor);

            if (!it_is_complete) 
                return;
            
            it_removes_itself();
        }

        private void it_removes_itself()
        {
            the_agenda.Removes(this);
            
            if (the_possible_tasks_holder.has_a_Value(out var the_tasks_holder))
            {
                var the_removed_task = the_tasks_holder.Removes_a_Task();
                (the_removed_task == this).Must_Be_True();
            }
            
            it_is_complete = true;
        }

        public override string ToString() => its_variant.ToString();

        private readonly ITaskVariant<TExecutor> its_variant;
        private readonly TExecutor the_executor;
        private readonly WAgenda the_agenda;
        private readonly Possible<WTasksHolder> the_possible_tasks_holder;
        
        private bool it_is_complete;
    }
}
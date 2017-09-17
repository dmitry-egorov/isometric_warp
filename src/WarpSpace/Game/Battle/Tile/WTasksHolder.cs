using C5;
using Lanski.Structures;
using WarpSpace.Game.Battle.Unit;

namespace WarpSpace.Game.Battle.Tile
{
    public class WTasksHolder
    {
        public WTasksHolder()
        {
            its_queued_tasks = new CircularQueue<WAgenda.Task>();
        }

        public Possible<WAgenda.Task> s_Last_Task => 
            its_queued_tasks.Count > 0 ? 
                its_queued_tasks[its_queued_tasks.Count - 1] : 
                Possible.Empty<WAgenda.Task>()
        ;

        public void Completes(WAgenda.Task the_task)
        {
            var the_completed_task = its_queued_tasks.Dequeue();
            
            (the_task == the_completed_task).Must_Be_True();
        }

        public void Adds(WAgenda.Task the_task)
        {
            its_queued_tasks.Enqueue(the_task);
        }

        private readonly CircularQueue<WAgenda.Task> its_queued_tasks;
    }
}
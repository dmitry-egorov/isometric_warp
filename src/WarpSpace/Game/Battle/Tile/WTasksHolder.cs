using C5;
using Lanski.Structures;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Game.Battle.Unit.Tasks;
using WarpSpace.Game.Tasks;

namespace WarpSpace.Game.Battle.Tile
{
    public class WTasksHolder
    {
        public WTasksHolder()
        {
            its_queued_tasks = new CircularQueue<ITask>();
        }

        public Possible<ITask> s_Last_Task => 
            its_queued_tasks.Count > 0 ? 
                its_queued_tasks[its_queued_tasks.Count - 1].as_a_Possible() : 
                Possible.Empty<ITask>()
        ;

        public ITask Removes_a_Task() => its_queued_tasks.Dequeue();

        public void Adds(ITask the_task)
        {
            its_queued_tasks.Enqueue(the_task);
        }

        private readonly CircularQueue<ITask> its_queued_tasks;
    }
}
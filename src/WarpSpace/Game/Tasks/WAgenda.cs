using System;
using C5;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Game.Battle.Unit.Tasks;

namespace WarpSpace.Game.Tasks
{
    public class WAgenda
    {
        public WAgenda()
        {
            its_queue = new CircularQueue<ITask>(16, MemoryType.Strict);
            its_changes_stream = new Stream<Change>();
        }

        public int s_Tasks_Count => its_queue.Count;
        public Possible<ITask> s_Possible_Last_Task => its_queue.Count > 0 ? its_queue[its_queue.Count - 1].as_a_Possible() : Possible.Empty<ITask>();
        public Possible<ITask> s_Possible_Next_Task => it_has_a_task ? its_queue[0].as_a_Possible() : Possible.Empty<ITask>();
        public bool has_a_Task => it_has_a_task;
        public IStream<Change> Changed => its_changes_stream;


        public Possible<T> s_Last_Task_Where<T>(Func<ITask, Possible<T>> matcher)
        {
            for (var i = its_queue.Count - 1; i >= 0; i--)
            {
                var the_task= its_queue[i];
                var match = matcher(the_task);
                if (match.has_a_Value())
                    return match;
            }
            
            return Possible.Empty<T>();
        }
        
        public void Updates()
        {
            if (this.its_has_the_next_task(out var the_next_task))
                the_next_task.Performs_a_Step();
        }

        public void Schedules_a_Task(ITask the_new_task)
        {
            its_queue.Enqueue(the_new_task);
            
            its_changes_stream.Next(Change.Create.Enqueued(the_new_task));
        }
        
        public void Removes(ITask the_task)
        {
            var the_current_task = its_queue.Dequeue();
            (the_current_task == the_task).Must_Be_True();
            
            its_changes_stream.Next(Change.Create.Completed(the_task));
        }
        
        private bool its_has_the_next_task(out ITask the_next_task) => s_Possible_Next_Task.has_a_Value(out the_next_task);

        private bool it_has_a_task => its_queue.Count > 0;

        private readonly CircularQueue<ITask> its_queue;
        private readonly Stream<Change> its_changes_stream;

        public struct Change
        {
            public static class Create
            {
                public static Change Enqueued(ITask the_task) => new Change(the_task, new Scheduled());
                public static Change Completed(ITask the_task) => new Change(the_task, new Completed());
            }

            private struct Scheduled { public override string ToString() => nameof(Scheduled); }
            private struct Completed { public override string ToString() => nameof(Completed); }

            private Change(ITask the_task, Or<Scheduled, Completed> the_variant)
            {
                its_task = the_task;
                its_variant = the_variant;
            }

            public bool is_a_Task_Scheduling(out ITask the_task) { the_task = its_task; return its_variant.is_a_T1(); }
            public bool is_a_Task_Completion(out ITask the_task) { the_task = its_task; return its_variant.is_a_T2(); }

            public override string ToString() => $"{its_variant} {its_task}";

            private readonly ITask its_task;
            private readonly Or<Scheduled, Completed> its_variant;
        }
    }
}
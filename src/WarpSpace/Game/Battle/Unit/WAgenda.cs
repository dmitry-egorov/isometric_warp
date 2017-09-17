using C5;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Game.Battle.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Game.Battle.Unit
{
    public class WAgenda
    {
        public WAgenda(WUnit the_owner, WBoard the_board)
        {
            this.the_board = the_board;
            its_owner = the_owner;
            its_queue = new CircularQueue<Task>(16, MemoryType.Strict);
            its_changes_stream = new Stream<Change>();


            its_changes_stream.Subscribe(e => 
                Debug.Log($"{its_owner.s_Unit.s_Name} {e}. " + 
                          $"Remaining: {its_queue.Count}. " + 
                          $"Next task: {its_possible_next_task.Select(c => c.s_Task)}")
            );
        }

        public bool has_a_Task => it_has_a_task;
        public IStream<Change> Changed => its_changes_stream;

        public bool s_Next_Task(out Task the_next_task) => its_possible_next_task.has_a_Value(out the_next_task);
        
        public void Enqueues_a_Move(MUnitLocation the_source, MUnitLocation the_target)
        {
            var the_source_tile = the_source.s_Tile;
            var the_target_tile = the_target.s_Tile;

            var the_orientation = the_source_tile.s_Direction_To(the_target_tile);
            var the_source_position = the_source_tile.s_Position;
            var the_target_position = the_target_tile.s_Position;
            
            if (the_source.is_a_Bay(out var the_source_bay))
            {
                Debug.Log("Undock");
                it_undocks_from(the_source_bay);
            }
            else if (the_target.is_a_Bay(out var the_target_bay))
            {
                Debug.Log("Dock");
                it_docks_to(the_target_bay);
            }
            else
            {
                Debug.Log("Move");
                it_moves();
            }

            void it_undocks_from(MBay the_bay)
            {
                var the_owner = the_board.s_Unit_For(the_bay.s_Owner).s_Agenda;

                var the_owners_rotation = the_owner.it_rotates_to(the_orientation);
                it_waits_for(the_owners_rotation);
                it_shows_up_at(the_source_position, the_orientation);
                var the_movement = it_moves_to(the_target_position);
                the_owner.it_waits_for(the_movement);
            }

            void it_docks_to(MBay the_bay)
            {
                var the_owner = the_board.s_Unit_For(the_bay.s_Owner).s_Agenda;

                var the_owners_rotation = the_owner.it_rotates_to(the_orientation.s_Opposite());
                it_rotates_to(the_orientation);
                it_waits_for(the_owners_rotation);
                it_moves_to(the_target_position);
                var the_hiding = it_hides();
                the_owner.it_waits_for(the_hiding);
            }

            void it_moves()
            {
                it_rotates_to(the_orientation);
                it_moves_to(the_target_position);
            }
        }


        private Direction2D its_expected_orientation
        {
            get
            {
                for (var i = its_queue.Count - 1; i >= 0; i--)
                {
                    var the_task = its_queue[i].s_Task;
                    if (the_task.is_to_Rotate_To(out var the_rotation) ||
                        the_task.is_to_Show_Up_At(out var _, out the_rotation))
                        return the_rotation;
                }

                return its_owner.s_Orientation.s_Value_Or(Direction2D.Left);
            }
        }

        private Possible<Index2D> its_possible_expected_position
        {
            get
            {
                for (var i = its_queue.Count - 1; i >= 0; i--)
                {
                    var the_task = its_queue[i].s_Task;
                    if (the_task.is_to_Move_To(out var the_position) || 
                        the_task.is_to_Show_Up_At(out the_position, out var _))
                        return the_position;
                }

                return its_owner.s_Position;
            }
        }

        private Possible<Task> it_moves_to(Index2D the_target_position)
        {
            var the_task_at_the_target = the_board[the_target_position].s_Tasks_Holder.s_Last_Task;
            it_waits_for(the_task_at_the_target);
            return it_schedules_a_task(DTask.Create.Movement_To(the_target_position));
        }

        private Possible<Task> it_rotates_to(Direction2D the_rotation) => 
            its_expected_orientation != the_rotation ? 
                it_schedules_a_task(DTask.Create.Rotation_To(the_rotation)) : 
                its_possible_last_task
        ;

        private Possible<Task> it_waits_for(Possible<Task> the_possible_task) => 
            the_possible_task.has_a_Value(out var the_task) ? 
                it_schedules_a_task(DTask.Create.Waiting_For(the_task)) : 
                Possible.Empty<Task>()
        ;
        private Task it_hides() => it_schedules_a_task(DTask.Create.Hiding());
        private Task it_shows_up_at(Index2D the_parent, Direction2D the_rotation) => it_schedules_a_task(DTask.Create.Showing_At(the_parent, the_rotation));
        private Task it_schedules_a_task(DTask the_task)
        {
            var the_possible_tile_tasks_holder = its_possible_expected_position.Converted_With(p => the_board[p].s_Tasks_Holder);
            var the_new_continer = new Task(the_task, this, the_possible_tile_tasks_holder);
            the_possible_tile_tasks_holder.Does(h => h.Adds(the_new_continer));
            
            its_queue.Enqueue(the_new_continer);
            
            its_changes_stream.Next(Change.Create.Enqueued(the_task));

            return the_new_continer;
        }

        
        private void it_completes(Task the_task)
        {
            var the_current_task = its_queue.Dequeue();
            (the_current_task == the_task).Must_Be_True();
            
            its_changes_stream.Next(Change.Create.Reached(the_task.s_Task));
        }

        private Possible<Task> its_possible_last_task => its_queue.Count > 0 ? its_queue[its_queue.Count - 1] : Possible.Empty<Task>();
        private Possible<Task> its_possible_next_task => it_has_a_task ? its_queue[0] : Possible.Empty<Task>();
        private bool it_has_a_task => its_queue.Count > 0;

        private readonly WUnit its_owner;
        private readonly WBoard the_board;
        private readonly CircularQueue<Task> its_queue;
        private readonly Stream<Change> its_changes_stream;

        public class Task
        {
            internal Task(DTask the_its_task, WAgenda the_agenda, Possible<WTasksHolder> the_tasks_holder)
            {
                its_task = the_its_task;
                this.the_agenda = the_agenda;
                this.the_tasks_holder = the_tasks_holder;
            }

            public bool is_Complete => it_is_complete;
            public DTask s_Task => its_task;

            public void Completes()
            {
                the_agenda.it_completes(this);
                the_tasks_holder.Does(h => h.Completes(this));
                it_is_complete = true;
            }

            private readonly DTask its_task;
            private readonly WAgenda the_agenda;
            private readonly Possible<WTasksHolder> the_tasks_holder;
            private bool it_is_complete;
        }
        
        public struct DTask
        {
            public static class Create
            {
                public static DTask Movement_To(Index2D the_position) => new DTask(new Movement {s_Position = the_position});
                public static DTask Rotation_To(Direction2D the_rotation) => new DTask(new Rotation {s_Rotation = the_rotation});
                public static DTask Hiding() => new DTask(new Hiding());
                public static DTask Showing_At(Index2D the_position, Direction2D the_rotation) => new DTask(new Showing {s_Position = the_position, s_Rotation = the_rotation});
                public static DTask Waiting_For(Task the_task) => new DTask(new WaitingForTask{s_Task = the_task});
            }

            private DTask(Or<Movement, Rotation, Hiding, Showing, WaitingForTask> the_its_variant)
            {
                its_variant = the_its_variant;
            }

            public bool is_to_Move() => its_variant.is_a_T1();
            public bool is_to_Move_To(out Index2D the_position) => its_variant.as_a_T1().Select(x => x.s_Position).has_a_Value(out the_position);
            public bool is_to_Rotate() => its_variant.is_a_T2();
            public bool is_to_Rotate_To(out Direction2D the_rotation) => its_variant.as_a_T2().Select(x => x.s_Rotation).has_a_Value(out the_rotation);

            public bool is_to_Hide() => its_variant.is_a_T3();

            public bool is_to_Show_Up() => its_variant.is_a_T4();

            public bool is_to_Show_Up_At(out Index2D the_parent, out Direction2D the_rotation)
            {
                if (!its_variant.is_a_T4(out var the_movement))
                {
                    the_parent = default(Index2D);
                    the_rotation = default(Direction2D);
                    return false;
                }

                the_parent = the_movement.s_Position;
                the_rotation = the_movement.s_Rotation;

                return true;
            }

            public bool is_to_Wait_For_a_Task(out Task the_task) => its_variant.as_a_T5().Select(x => x.s_Task).has_a_Value(out the_task);

            private readonly Or<Movement, Rotation, Hiding, Showing, WaitingForTask> its_variant;

            private struct Movement
            {
                public Index2D s_Position;
                public override string ToString() => $"{nameof(Movement)} to {s_Position}"; 
            }

            private struct Rotation
            {
                public Direction2D s_Rotation;
                public override string ToString() => $"{nameof(Rotation)} to {s_Rotation}";
            }
            private struct Hiding {public override string ToString() => nameof(Hiding);}
            private struct Showing
            {
                public Index2D s_Position; 
                public Direction2D s_Rotation;
                public override string ToString() => $"{nameof(Showing)} at {s_Position}, {s_Rotation}";
            }

            private struct WaitingForTask
            {
                public Task s_Task;
                public override string ToString() => $"{nameof(WaitingForTask)} for {s_Task.s_Task}";
            }

            public override string ToString() => its_variant.ToString();
        }

        public struct Change
        {
            public static class Create
            {
                public static Change Enqueued(DTask the_task) => new Change(the_task, new Scheduled());
                public static Change  Reached(DTask the_task) => new Change(the_task, new Completed());
            }

            private struct Scheduled { public override string ToString() => nameof(Scheduled); }
            private struct Completed { public override string ToString() => nameof(Completed); }

            private Change(DTask the_task, Or<Scheduled, Completed> the_variant)
            {
                its_task = the_task;
                its_variant = the_variant;
            }

            public bool is_a_Task_Scheduling(out DTask the_task) { the_task = its_task; return its_variant.is_a_T1(); }
            public bool is_a_Task_Completion(out DTask the_task) { the_task = its_task; return its_variant.is_a_T2(); }

            public override string ToString() => $"{its_variant} {its_task}";

            private readonly DTask its_task;
            private readonly Or<Scheduled, Completed> its_variant;
        }
    }
}
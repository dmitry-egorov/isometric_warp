using System;
using C5;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Game.Battle.Unit
{
    public class WAgenda
    {
        public WAgenda(WUnit the_its_owner, WBoard the_board)
        {
            this.the_board = the_board;
            its_owner = the_its_owner;
            its_queue = new CircularQueue<TaskContainer>(16, MemoryType.Strict);
            its_movement_events_stream = new Stream<Change>();

            its_wiring = it_wires_units_movements();
        }

        public bool has_a_Task => it_has_a_task();
        public IStream<Change> s_Changes_Stream => its_movement_events_stream;

        public void Destructs() => it_destructs();

        public bool s_Next_Task(out Task the_next_task) => its_possible_task().has_a_Value(out the_next_task);
        public void Completes_the_Current_Task() => it_completes_the_current_task(); 

        private Action it_wires_units_movements() => its_owner.s_Unit.Moved.Subscribe(x => it_enqueues_a_move(x.Source, x.Destination));
        
        private void it_destructs() => its_wiring();

        private Possible<TaskContainer> its_last_task_container() => its_queue.Count == 0 ? Possible.Empty<TaskContainer>() : its_queue[its_queue.Count - 1];

        private Direction2D its_expected_rotation()
        {
            for (var i = its_queue.Count - 1; i >= 0; i--)
            {
                var the_task = its_queue[i].s_Task;
                if (the_task.is_to_Rotate_To(out var the_rotation) || the_task.is_to_Show_At(out var _, out the_rotation))
                    return the_rotation;
            }

            return its_owner.s_Tarnsform.localRotation.s_Direction();
        }

        private void it_enqueues_a_move(MUnitLocation the_source, MUnitLocation the_target)
        {
            var the_source_tile = the_source.s_Tile;
            var the_target_tile = the_target.s_Tile;

            var the_orientation = the_source_tile.s_Direction_To(the_target_tile);
            var the_source_transform = transform_of(the_source_tile);
            var the_target_transform = transform_of(the_target_tile);
            
            if (the_source.is_a_Bay(out var the_source_bay))
            {
                it_undocks(the_source_bay);
            }
            else if (the_target.is_a_Bay(out var the_target_bay))
            {
                it_docks(the_target_bay);
            }
            else
            {
                it_moves();
            }

            void it_undocks(MBay the_bay)
            {
                var the_owner = the_board.s_Unit_For(the_bay.s_Owner).s_Agenda;

                var the_owners_rotation = the_owner.it_rotates_to(the_orientation);
                it_waits_for(the_owners_rotation);
                it_shows_up_at(the_source_transform, the_orientation);
                var the_movement = it_moves_to(the_target_transform);
                the_owner.it_waits_for(the_movement);
            }

            void it_docks(MBay the_bay)
            {
                var the_owner = the_board.s_Unit_For(the_bay.s_Owner).s_Agenda;

                var the_owners_rotation = the_owner.it_rotates_to(the_orientation.s_Opposite());
                it_rotates_to(the_orientation);
                it_waits_for(the_owners_rotation);
                it_moves_to(the_target_transform);
                var the_hiding = it_hides();
                the_owner.it_waits_for(the_hiding);
            }

            void it_moves()
            {
                it_rotates_to(the_orientation);
                it_moves_to(the_target_transform);
            }

            Transform transform_of(MTile the_tile) => the_board[the_tile].s_UnitSlot.Transform;
        }

        private Possible<TaskContainer> it_moves_to(Transform the_parent) => it_schedules_a_task(Task.Create.Movement(the_parent));

        private Possible<TaskContainer> it_rotates_to(Direction2D the_rotation) => 
            its_expected_rotation() != the_rotation ? 
                it_schedules_a_task(Task.Create.Rotation(the_rotation)) : 
                Possible.Empty<TaskContainer>()
        ;

        private Possible<TaskContainer> it_waits_for(Possible<TaskContainer> the_possible_task) => 
            the_possible_task.has_a_Value(out var the_task) ? 
                it_schedules_a_task(Task.Create.Waiting(the_task)) : 
                Possible.Empty<TaskContainer>()
        ;
        private TaskContainer it_hides() => it_schedules_a_task(Task.Create.Hiding());
        private TaskContainer it_shows_up_at(Transform the_parent, Direction2D the_rotation) => it_schedules_a_task(Task.Create.Showing(the_parent, the_rotation));
        private TaskContainer it_schedules_a_task(Task the_task) 
        {
            if (its_last_task_container().has_a_Value(out var the_last_container) && the_last_container.s_Task.has_the_same_Type_As(the_task))
            {
                the_last_container.Replaces_the_Task_With(the_task);
                
                its_movement_events_stream.Next(Change.Create.Replaced(the_task));
                Debug.Log($"Replaced movement for {its_owner.s_Unit.s_Type.s_Total_Hit_Points}! Remaining: {its_queue.Count}");

                return the_last_container;
            }
            
            var the_new_continer = new TaskContainer(the_task);
            its_queue.Enqueue(the_new_continer);
            
            its_movement_events_stream.Next(Change.Create.Enqueued(the_task));
            Debug.Log($"Scheduled a task for {its_owner.s_Unit.s_Type.s_Total_Hit_Points}! Remaining: {its_queue.Count}");

            return the_new_continer;
        }
        
        private void it_completes_the_current_task()
        {
            var the_task = its_queue.Dequeue();
            the_task.Completes();
            its_movement_events_stream.Next(Change.Create.Reached(the_task.s_Task));
            Debug.Log($"A task of {its_owner.s_Unit.s_Type.s_Total_Hit_Points} is complete! Remaining: {its_queue.Count}");
        }

        private bool it_doesnt_have_a_task(out Task the_task) => !it_has_a_task(out the_task);
        private bool it_has_a_task(out Task the_task) => its_possible_task().has_a_Value(out the_task);
        private Possible<Task> its_possible_task() => it_has_a_task() ? its_queue[0].s_Task : Possible.Empty<Task>();
        private bool it_has_a_task() => its_queue.Count > 0;

        private bool it_has_a_pending_task() => its_queue.Count > 1;
        private bool it_has_a_pending_task(out Task the_pending_task)
        {
            the_pending_task = default(Task);
            if (!it_has_a_pending_task()) 
                return false;
                
            the_pending_task = its_queue[1].s_Task;
            return true;
        }

        private readonly WUnit its_owner;
        private readonly WBoard the_board;
        private readonly Action its_wiring;
        private readonly CircularQueue<TaskContainer> its_queue;
        private readonly Stream<Change> its_movement_events_stream;

        public class TaskContainer
        {
            internal TaskContainer(Task the_its_task) => its_task = the_its_task;

            public bool is_Complete => it_is_complete;
            public Task s_Task => its_task;

            public void Completes() => it_is_complete = true;
            
            private Task its_task;
            private bool it_is_complete;

            public void Replaces_the_Task_With(Task the_task)
            {
                its_task = the_task;
            }
        }
        
        public struct Task
        {
            public static class Create
            {
                public static Task Movement(Transform the_parent) => new Task(new Movement {s_Parent = the_parent});
                public static Task Rotation(Direction2D the_rotation) => new Task(new Rotation {s_Rotation = the_rotation});
                public static Task Hiding() => new Task(new Hiding());
                public static Task Showing(Transform the_parent, Direction2D the_rotation) => new Task(new Showing {s_Parent = the_parent, s_Rotation = the_rotation});
                public static Task Waiting(TaskContainer the_task_container) => new Task(new Waiting{s_Task_Container = the_task_container});
            }

            private Task(Or<Movement, Rotation, Hiding, Showing, Waiting> the_its_variant)
            {
                its_variant = the_its_variant;
            }

            public bool is_to_Move() => its_variant.is_a_T1();
            public bool is_to_Move_To(out Transform the_parent) => its_variant.as_a_T1().Select(x => x.s_Parent).has_a_Value(out the_parent);
            public bool is_to_Rotate_To(out Direction2D the_rotation) => its_variant.as_a_T2().Select(x => x.s_Rotation).has_a_Value(out the_rotation);

            public bool is_to_Hide() => its_variant.is_a_T3();

            public bool is_to_Show() => its_variant.is_a_T4();

            public bool is_to_Show_At(out Transform the_parent, out Direction2D the_rotation)
            {
                if (!its_variant.is_a_T4(out var the_movement))
                {
                    the_parent = null;
                    the_rotation = default(Direction2D);
                    return false;
                }

                the_parent = the_movement.s_Parent;
                the_rotation = the_movement.s_Rotation;

                return true;
            }

            public bool is_to_Wait_For(out TaskContainer the_task_container) => its_variant.as_a_T5().Select(x => x.s_Task_Container).has_a_Value(out the_task_container);

            private readonly Or<Movement, Rotation, Hiding, Showing, Waiting> its_variant;

            private struct Movement
            {
                public Transform s_Parent; 
            }

            private struct Rotation
            {
                public Direction2D s_Rotation;
            }
            private struct Hiding {}
            private struct Showing
            {
                public Transform s_Parent; 
                public Direction2D s_Rotation;
            }

            private struct Waiting
            {
                public TaskContainer s_Task_Container;
            }

            public bool has_the_same_Type_As(Task the_task) => its_variant.has_the_Same_Type_As(the_task.its_variant);
        }

        public struct Change
        {
            public static class Create
            {
                public static Change Enqueued(Task the_task) => new Change(new Enqueued {s_Task = the_task});
                public static Change Reached(Task the_task) => new Change(new Reached {s_Task = the_task});
                public static Change Replaced(Task the_task) => new Change(new Replaced {s_Task = the_task});
            }
            
            private struct Enqueued { public Task s_Task; }
            private struct Reached { public Task s_Task; }
            private struct Replaced { public Task s_Task; }

            private Change(Or<Enqueued, Reached, Replaced> the_its_variant) => its_variant = the_its_variant;
            
            public bool is_Enqueued_a_Task() => its_variant.is_a_T1();
            public bool is_Completed_a_Task() => its_variant.is_a_T2();
            public bool is_Replaced_a_Task() => its_variant.is_a_T3();

            public bool is_Enqueued_a_Task(out Task the_task) => its_variant.as_a_T1().Select(x => x.s_Task).has_a_Value(out the_task);
            public bool is_Completed_a_Task(out Task the_task) => its_variant.as_a_T2().Select(x => x.s_Task).has_a_Value(out the_task);
            public bool is_Replaced_a_Task(out Task the_task) => its_variant.as_a_T3().Select(x => x.s_Task).has_a_Value(out the_task);

            private readonly Or<Enqueued, Reached, Replaced> its_variant;
        }
    }
}
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
    public class WMovementQueue
    {
        public WMovementQueue(WUnit the_owner, Transform the_limbo, WBoard the_board)
        {
            this.the_board = the_board;
            this.the_limbo = the_limbo;
            its_queue = new CircularQueue<TargetLocation>(16, MemoryType.Strict);
            its_movement_events_stream = new Stream<MovementEvent>();

            its_wiring = it_wires_the_movements();

            Action it_wires_the_movements() => the_owner.s_Unit.s_Movements_Stream.Subscribe(x => it_enqueues_a_move(x.Source, x.Destination));
        }
        
        public bool has_a_Target => it_has_a_target();
        public IStream<MovementEvent> s_Movement_Events_Stream => its_movement_events_stream;

        public void Destructs() => it_destructs();

        public Possible<TargetLocation> Gives_the_Next_Target()
        {
            it_updates_the_queue();

            return its_possible_target();
        }

        public void Removes_the_Current_Target() => it_removes_the_current_target(); 
        
        private void it_destructs() => its_wiring();
        
        private void it_enqueues_a_move(MUnitLocation source, MUnitLocation target)
        {
            var the_source_tile = source.s_Tile;
            var the_target_tile = target.s_Tile;

            var rotation = the_source_tile.Direction_To(the_target_tile).To_Rotation();
                    
            if (source.is_a_Bay())
            {
                it_enqueues_a_location(Transform_Of(the_source_tile), rotation, true);
            }

            it_enqueues_a_location(Transform_Of(the_target_tile), rotation, false);

            if (target.is_a_Bay())
            {
                it_enqueues_a_limbo();
            }

            Transform Transform_Of(MTile the_tile) => the_board[the_tile].s_UnitSlot.Transform;
        }

        private void it_enqueues_a_limbo() => it_enqueues_a_target(new TargetLocation(the_limbo, Quaternion.identity, true));
        private void it_enqueues_a_location(Transform the_parent, Quaternion the_rotation, bool is_teleport) => it_enqueues_a_target(new TargetLocation(the_parent, the_rotation, is_teleport));
        private void it_enqueues_a_target(TargetLocation the_target_location) 
        {
            its_queue.Enqueue(the_target_location);
            its_movement_events_stream.Next(MovementEvent.Create.MovingTowards(the_target_location));
        }

        private void it_updates_the_queue()
        {
            while 
            (
                it_has_a_pending_target(out var the_pending_target) &&
                (
                    it_doesnt_have_a_target(out var the_target)
                    ||
                    the_target.can_be_Merged_With(the_pending_target)
                )
            )
            {
                it_discards_the_current_target();
            }
        }

        private void it_discards_the_current_target()
        {
            var the_target = its_queue.Dequeue();
            its_movement_events_stream.Next(MovementEvent.Create.Discarded(the_target));
        }
        
        private void it_removes_the_current_target()
        {
            var the_target = its_queue.Dequeue();
            its_movement_events_stream.Next(MovementEvent.Create.Reched(the_target));
        }

        private bool it_doesnt_have_a_target(out TargetLocation current_target) => !it_has_a_target(out current_target);
        private bool it_has_a_target(out TargetLocation the_target) => its_possible_target().has_a_Value(out the_target);
        private Possible<TargetLocation> its_possible_target() => it_has_a_target() ? its_queue[0] : Possible.Empty<TargetLocation>();
        private bool it_has_a_target() => its_queue.Count > 0;

        private bool it_has_a_pending_target() => its_queue.Count > 1;
        private bool it_has_a_pending_target(out TargetLocation the_pending_target)
        {
            the_pending_target = default(TargetLocation);
            if (!it_has_a_pending_target()) 
                return false;
                
            the_pending_target = its_queue[1];
            return true;
        }
        
        private readonly WBoard the_board;
        private readonly Transform the_limbo;
        private readonly Action its_wiring;
        private readonly CircularQueue<TargetLocation> its_queue;
        private readonly Stream<MovementEvent> its_movement_events_stream;

        public struct TargetLocation
        {
            public readonly Transform s_Parent; 
            public readonly Quaternion s_Rotation;
            public readonly bool is_Teleportation;
                
            public TargetLocation(Transform parent, Quaternion rotation, bool is_teleportation)
            {
                s_Parent = parent;
                s_Rotation = rotation;
                is_Teleportation = is_teleportation;
            }
            
            public bool can_be_Merged_With(TargetLocation the_other) =>
                s_Rotation == the_other.s_Rotation && !is_Teleportation && !the_other.is_Teleportation 
                || is_Teleportation && the_other.is_Teleportation
            ;
        }

        public struct MovementEvent
        {
            public static class Create
            {
                public static MovementEvent MovingTowards(TargetLocation the_target) => new MovementEvent(new Enqued {Target = the_target});
                public static MovementEvent Reched(TargetLocation the_target) => new MovementEvent(new Reached {Target = the_target});
                public static MovementEvent Discarded(TargetLocation the_target) => new MovementEvent(new Discarded {Target = the_target});
            }
            
            private struct Enqued { public TargetLocation Target; }
            private struct Reached { public TargetLocation Target; }
            private struct Discarded { public TargetLocation Target; }

            private MovementEvent(Or<Enqued, Reached, Discarded> the_its_variant) => its_variant = the_its_variant;
            
            public bool is_Teleported_To(out Transform the_parent_object)
            {
                if (is_Reached_a_Target(out var the_target) && the_target.is_Teleportation)
                {
                    the_parent_object = the_target.s_Parent;
                    return true;
                }

                the_parent_object = null;
                return false;
            }

            public bool is_Enqueued_a_Target(out TargetLocation the_target) => its_variant.as_a_T1().Select(x => x.Target).has_a_Value(out the_target);
            public bool is_Reached_a_Target(out TargetLocation the_target) => its_variant.as_a_T2().Select(x => x.Target).has_a_Value(out the_target);
            public bool is_Discarded_a_Target(out TargetLocation the_target) => its_variant.as_a_T3().Select(x => x.Target).has_a_Value(out the_target);

            private readonly Or<Enqued, Reached, Discarded> its_variant;
        }
    }
}
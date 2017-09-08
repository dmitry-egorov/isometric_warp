using System;
using System.Collections.Generic;
using Lanski.Geometry;
using Lanski.Structures;
using Lanski.SwiftLinq;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    [RequireComponent(typeof(WUnit))]
    public class WMover: MonoBehaviour
    {
        public void Start() => it.inits();
        public void Update() => it.updates();
        public void OnDestroy() => it.destructs();

        private void inits()
        {
            var the_unit = GetComponent<WUnit>().s_Unit;
            var the_settings_holder = FindObjectOfType<UnitSettingsHolder>();

            it.s_queue = new Queue<TargetLocation>(16);
            it.s_settings = the_settings_holder.For(the_unit.s_Type).Movement;
            it.s_transform = transform;
            the_limbo = FindObjectOfType<WLimbo>().s_Transform;
            the_board = FindObjectOfType<BoardComponent>();
            the_renderers = GetComponentsInChildren<MeshRenderer>();

            it.s_acceleration = the_acceleration_from(it.s_settings.MaxSpeed, it.s_settings.MinSpeed, it.s_settings.AccelerationDistance);
            it.s_angular_acceleration = the_acceleration_from(it.s_settings.MaxAngularSpeed, it.s_settings.MinAngularSpeed, it.s_settings.AnglularAccelerationDistance);
            
            it.s_wiring = Wire_Movements();

            Action Wire_Movements() => the_unit.s_Movements_Stream.Subscribe(x => it.enqueues_a_move(x.Source, x.Destination));

            float the_acceleration_from(float maxSpeed, float minSpeed, float accelerationDistance)
            {
                var v0 = maxSpeed;
                var v1 = minSpeed;
                var d = accelerationDistance;

                return Mathf.Abs((v1 * v1 - v0 * v0) / (2f * d));
            }
        }
        
        void enqueues_a_move(MLocation source, MLocation target)
        {
            var the_source_tile = source.s_Tile;
            var the_target_tile = target.s_Tile;

            var rotation = the_source_tile.Direction_To(the_target_tile).To_Rotation();
                    
            if (source.is_a_Bay())
            {
                it.enqueues_a_location(Transform_Of(the_source_tile), rotation, true);
            }

            it.enqueues_a_location(Transform_Of(the_target_tile), rotation, false);

            if (target.is_a_Bay())
            {
                it.enqueues_a_limbo();
            }

            Transform Transform_Of(MTile the_tile) => the_board[the_tile].UnitSlot.Transform;
        }

        private void enqueues_a_limbo() => it.s_queue.Enqueue(new TargetLocation(the_limbo, Vector3.zero, Quaternion.identity, true));
        private void enqueues_a_location(Transform the_parent, Quaternion the_rotation, bool is_teleport) => it.s_queue.Enqueue(new TargetLocation(the_parent, Vector3.zero, the_rotation, is_teleport));
        
        private void updates()
        {
            it.disables_rendering_in_limbo();
            it.updates_the_target();

            if (it.doesnt_have_a_target(out var the_target))
                return;

            if (the_target.is_Teleportation)
            {
                it.teleports_to(the_target);
            }
            else
            {
                it.moves_to(the_target);
            }
        }
        
        private void destructs() => it.s_wiring();

        private void disables_rendering_in_limbo()
        {
            var it_should_render = it.s_transform.parent != the_limbo;
            var iterator = the_renderers.SIterate();
            while (iterator.has_a_Value(out var the_renderer))
            {
                the_renderer.enabled = it_should_render;
            }
        }

        private void updates_the_target()
        {
            while (it.has_a_pending_target(out var the_pending_target) && (!it.has_a_target(out var the_target) || the_target.can_be_Merged_With(the_pending_target)))
                it.dequeues_a_target();
        }
        
        private bool has_a_pending_target(out TargetLocation the_pending_target)
        {
            the_pending_target = default(TargetLocation);
            if (it.s_queue.Count == 0) 
                return false;
                
            the_pending_target = it.s_queue.Peek();
            return true;
        }
        
        private void dequeues_a_target() => it.s_current_target = it.s_queue.Dequeue();

        private void moves_to(TargetLocation the_target_tile)
        {
            var parent = the_target_tile.s_Parent;
            var dt = Time.deltaTime;
            var tr = the_target_tile.s_Rotation;
            var tp = the_target_tile.s_Position;
                
            updates_the_parent();
                
            var r = it.s_transform.localRotation;
            var p = it.s_transform.localPosition;

            updates_objects_rotation();
            updates_objects_position();
            checks_if_target_Is_reached();

            void updates_the_parent()
            {
                if (it.s_transform.parent == parent) 
                    return;
                
                it.s_transform.parent = parent;
            }

            void updates_objects_rotation()
            {
                var dr = remaining_angle();

                if (dr == 0f)
                    return;

                var s = it.s_angular_speed;
                var a = it.s_angular_acceleration;
                var maxs = it.s_settings.MaxAngularSpeed;
                var mins = it.s_settings.MinAngularSpeed;
                var maxdr = it.s_settings.AnglularAccelerationDistance;
                    
                var ts = the_target_speed(); 
                it.s_angular_speed = s = new_rotation_speed();
                it.s_transform.localRotation = r = new_rotation();

                float the_target_speed() => dr > maxdr ? maxs : mins;
                float new_rotation_speed() => Mathf.MoveTowards(s, ts, a * dt);
                Quaternion new_rotation() => Quaternion.RotateTowards(r, tr, s * dt);
            }

            void updates_objects_position()
            {
                var dr = remaining_angle();
                var dp = remaining_distance();
                    
                if (dr != 0.0f) 
                    return;
                if (dp == 0.0f) 
                    return;
                    
                var s = it.s_speed;
                var a = it.s_acceleration;
                var maxs = it.s_settings.MaxSpeed;
                var mins = it.s_settings.MinSpeed;
                var maxdp = it.s_settings.AccelerationDistance;

                var ts = the_target_speed();
                it.s_speed = s = new_speed();
                it.s_transform.localPosition = p = new_position();

                float the_target_speed() => dp > maxdp ? maxs : mins; 
                float new_speed() => Mathf.MoveTowards(s, ts, a * dt);
                Vector3 new_position() => Vector3.MoveTowards(p, tp, s * dt);
            }

            void checks_if_target_Is_reached()
            {
                var dr = remaining_angle();
                var dp = remaining_distance();
                    
                if (dp != 0f) 
                    return;
                if (dr != 0f) 
                    return;
                    
                resets_the_target();
                Update();
            }

            float remaining_angle() => Quaternion.Angle(r, tr);
            float remaining_distance() => p.DistanceTo(tp);
        }

        private bool has_a_target(out TargetLocation current_target) => it.s_current_target.has_a_Value(out current_target);
        private bool doesnt_have_a_target(out TargetLocation current_target) => !has_a_target(out current_target);

        private void teleports_to(TargetLocation the_target)
        {
            it.s_speed = 0;
            it.s_angular_speed = 0;
            it.s_transform.parent = the_target.s_Parent;
            it.s_transform.localPosition = the_target.s_Position;
            it.s_transform.localRotation = the_target.s_Rotation;
            it.resets_the_target();
        }

        private void resets_the_target() => it.s_current_target = Possible.Empty<TargetLocation>();

        private WMover it => this;
        
        private MovementSettings s_settings;
        private Transform s_transform;
        private Transform the_limbo;
        private BoardComponent the_board;
        private IReadOnlyList<MeshRenderer> the_renderers;
        private Action s_wiring;

        private Queue<TargetLocation> s_queue;
        private float s_acceleration;
        private float s_angular_acceleration;

        private Possible<TargetLocation> s_current_target;
        private float s_speed;
        private float s_angular_speed;

        private struct TargetLocation
        {
            public readonly Transform s_Parent; 
            public readonly Vector3 s_Position;
            public readonly Quaternion s_Rotation;
            public readonly bool is_Teleportation;
                
            public TargetLocation(Transform parent, Vector3 position, Quaternion rotation, bool is_teleportation)
            {
                s_Parent = parent;
                s_Position = position;
                s_Rotation = rotation;
                is_Teleportation = is_teleportation;
            }
            
            public bool can_be_Merged_With(TargetLocation the_other) =>
                this.s_Rotation == the_other.s_Rotation &&
                this.is_Teleportation == the_other.is_Teleportation
            ;
        }
    }
}
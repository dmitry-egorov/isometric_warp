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
        public float BoostSpeedMultiplier; 
        
        public bool is_Moving => it_has_a_target() || it_has_a_pending_target();
        
        public void Fast_Forwards() => it_is_fast_forwarding = true;
        public void Resumes_Normal_Speed() => it_is_fast_forwarding = false;

        public void Start() => it_inits();
        public void Update() => it_updates();
        public void OnDestroy() => it_destructs();

        private void it_inits()
        {
            var the_unit = GetComponent<WUnit>().s_Unit;

            its_queue = new Queue<TargetLocation>(16);
            its_settings = UnitTypeSettings.Of(the_unit.s_Type).Movement;
            its_transform = transform;
            the_limbo = FindObjectOfType<WLimbo>().s_Transform;
            the_board = FindObjectOfType<WBoard>();
            the_renderers = GetComponentsInChildren<MeshRenderer>();

            its_acceleration = the_acceleration_from(its_settings.MaxSpeed, its_settings.MinSpeed, its_settings.AccelerationDistance);
            its_angular_acceleration = the_acceleration_from(its_settings.MaxAngularSpeed, its_settings.MinAngularSpeed, its_settings.AnglularAccelerationDistance);

            its_transform.localRotation = Direction2D.Left.To_Rotation();
            
            it_disables_rendering_if_needed();
            
            its_wiring = Wire_Movements();

            Action Wire_Movements() => the_unit.s_Movements_Stream.Subscribe(x => this.enqueues_a_move(x.Source, x.Destination));

            float the_acceleration_from(float max_speed, float min_speed, float acceleration_distance)
            {
                var v0 = max_speed;
                var v1 = min_speed;
                var d = acceleration_distance;

                return Mathf.Abs((v1 * v1 - v0 * v0) / (2f * d));
            }
        }

        private bool it_has_a_pending_target() => its_queue.Count != 0;
        private bool it_has_a_pending_target(out TargetLocation the_pending_target)
        {
            the_pending_target = default(TargetLocation);
            if (!it_has_a_pending_target()) 
                return false;
                
            the_pending_target = its_queue.Peek();
            return true;
        }

        private bool it_has_a_target() => its_current_target.has_a_Value();
        private bool it_has_a_target(out TargetLocation current_target) => its_current_target.has_a_Value(out current_target);
        private bool it_doesnt_have_a_target(out TargetLocation current_target) => !it_has_a_target(out current_target);
        private bool it_is_in_limbo() => its_transform.parent == the_limbo;
        
        private void enqueues_a_move(MUnitLocation source, MUnitLocation target)
        {
            var the_source_tile = source.s_Tile;
            var the_target_tile = target.s_Tile;

            var rotation = the_source_tile.Direction_To(the_target_tile).To_Rotation();
                    
            if (source.is_a_Bay())
            {
                this.enqueues_a_location(Transform_Of(the_source_tile), rotation, true);
            }

            this.enqueues_a_location(Transform_Of(the_target_tile), rotation, false);

            if (target.is_a_Bay())
            {
                this.enqueues_a_limbo();
            }

            Transform Transform_Of(MTile the_tile) => the_board[the_tile].s_UnitSlot.Transform;
        }

        private void enqueues_a_limbo() => its_queue.Enqueue(new TargetLocation(the_limbo, Vector3.zero, Quaternion.identity, true));
        private void enqueues_a_location(Transform the_parent, Quaternion the_rotation, bool is_teleport) => its_queue.Enqueue(new TargetLocation(the_parent, Vector3.zero, the_rotation, is_teleport));
        
        private void it_updates()
        {
            this.updates_the_target();

            if (this.it_doesnt_have_a_target(out var the_target))
                return;

            if (the_target.is_Teleportation)
            {
                this.teleports_to(the_target);
            }
            else
            {
                this.moves_to(the_target);
            }
        }
        
        private void it_destructs() => its_wiring();

        private void updates_the_target()
        {
            while (this.it_has_a_pending_target(out var the_pending_target) && 
                   (
                       !this.it_has_a_target(out var the_target) 
                       || 
                       the_target.can_be_Merged_With(the_pending_target)
                   )
                )
                this.dequeues_a_target();
        }
        
        private void dequeues_a_target() => its_current_target = its_queue.Dequeue();

        private void moves_to(TargetLocation the_target_tile)
        {
            var parent = the_target_tile.s_Parent;
            var dt = Time.deltaTime;
            var tr = the_target_tile.s_Rotation;
            var tp = the_target_tile.s_Position;
            var boost = it_is_fast_forwarding ? BoostSpeedMultiplier : 1f;

            updates_the_parent();
                
            var r = its_transform.localRotation;
            var p = its_transform.localPosition;

            updates_objects_rotation();
            updates_objects_position();
            checks_if_target_Is_reached();

            void updates_the_parent()
            {
                if (its_transform.parent == parent) 
                    return;
                
                its_transform.parent = parent;
            }

            void updates_objects_rotation()
            {
                var dr = remaining_angle();

                if (dr == 0f)
                    return;

                var s = its_angular_speed;
                var a = its_angular_acceleration;
                var maxs = its_settings.MaxAngularSpeed;
                var mins = its_settings.MinAngularSpeed;
                var maxdr = its_settings.AnglularAccelerationDistance;
                    
                var ts = the_target_speed(); 
                its_angular_speed = s = new_rotation_speed();
                its_transform.localRotation = r = new_rotation();

                float the_target_speed() => dr > maxdr ? maxs : mins;
                float new_rotation_speed() => Mathf.MoveTowards(s, ts, a * dt);
                Quaternion new_rotation() => Quaternion.RotateTowards(r, tr, boost * s * dt);
            }

            void updates_objects_position()
            {
                var dr = remaining_angle();
                var dp = remaining_distance();
                    
                if (dr != 0.0f) 
                    return;
                if (dp == 0.0f) 
                    return;
                    
                var s = its_speed;
                var a = its_acceleration;
                var maxs = its_settings.MaxSpeed;
                var mins = its_settings.MinSpeed;
                var maxdp = its_settings.AccelerationDistance;

                var ts = the_target_speed();
                its_speed = s = new_speed();
                its_transform.localPosition = p = new_position();

                float the_target_speed() => dp > maxdp ? maxs : mins; 
                float new_speed() => Mathf.MoveTowards(s, ts, a * dt);
                Vector3 new_position() => Vector3.MoveTowards(p, tp, boost * s * dt);
            }

            void checks_if_target_Is_reached()
            {
                var dr = remaining_angle();
                var dp = remaining_distance();
                    
                if (dp != 0f) 
                    return;
                if (dr != 0f) 
                    return;
                    
                it_resets_the_target();
                Update();
            }

            float remaining_angle() => Quaternion.Angle(r, tr);
            float remaining_distance() => p.DistanceTo(tp);
        }

        private void teleports_to(TargetLocation the_target)
        {
            its_speed = 0;
            its_angular_speed = 0;
            its_transform.parent = the_target.s_Parent;
            its_transform.localPosition = the_target.s_Position;
            its_transform.localRotation = the_target.s_Rotation;
            
            it_disables_rendering_if_needed();

            it_resets_the_target();
        }

        private void it_disables_rendering_if_needed()
        {
            var iterator = the_renderers.s_New_Iterator();
            while (iterator.has_a_Value(out var the_renderer))
            {
                if (the_renderer != null)
                {
                    the_renderer.enabled = !it_is_in_limbo();
                }
            }
        }

        private void it_resets_the_target() => its_current_target = Possible.Empty<TargetLocation>();

        private MovementSettings its_settings;
        private Transform its_transform;
        private Transform the_limbo;
        private WBoard the_board;
        private IReadOnlyList<MeshRenderer> the_renderers;
        private Action its_wiring;

        private Possible<TargetLocation> its_current_target;
        private Queue<TargetLocation> its_queue;
        private float its_acceleration;
        private float its_angular_acceleration;

        private float its_speed;
        private float its_angular_speed;
        private bool it_is_fast_forwarding;

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
                this.s_Rotation == the_other.s_Rotation && !this.is_Teleportation && !the_other.is_Teleportation 
                || this.is_Teleportation && the_other.is_Teleportation
            ;
        }
    }
}
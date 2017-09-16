using Lanski.Geometry;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    public class WMover
    {
        public WMover(WMovementQueue the_queue, MovementSettings the_settings, float the_its_boost_speed_multiplier, Transform the_transform)
        {
            this.the_queue = the_queue;
            its_settings = the_settings;
            its_transform = the_transform;
            its_boost_speed_multiplier = the_its_boost_speed_multiplier;

            its_acceleration = the_acceleration_from(its_settings.MaxSpeed, its_settings.MinSpeed, its_settings.AccelerationDistance);
            its_angular_acceleration = the_acceleration_from(its_settings.MaxAngularSpeed, its_settings.MinAngularSpeed, its_settings.AnglularAccelerationDistance);

            its_transform.localRotation = Direction2D.Left.To_Rotation();
            
            float the_acceleration_from(float max_speed, float min_speed, float acceleration_distance)
            {
                var v0 = max_speed;
                var v1 = min_speed;
                var d = acceleration_distance;

                return Mathf.Abs((v1 * v1 - v0 * v0) / (2f * d));
            }
        }

        public void Fast_Forwards() => it_is_fast_forwarding = true;
        public void Resumes_Normal_Speed() => it_is_fast_forwarding = false;

        public void Updates() => it_updates();

        private void it_updates()
        {
            var the_possible_target = the_queue.Gives_the_Next_Target();
            if (the_possible_target.Does_Not_Have_a_Value(out var the_target))
                return;

            if (the_target.is_Teleportation)
            {
                it_teleports_to(the_target);
            }
            else
            {
                it_moves_to(the_target);
            }
        }

        private void it_moves_to(WMovementQueue.TargetLocation the_target)
        {
            var parent = the_target.s_Parent;
            var dt = Time.deltaTime;
            var tr = the_target.s_Rotation;
            var tp = Vector3.zero;
            var boost = it_is_fast_forwarding ? its_boost_speed_multiplier : 1f;

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
                    
                the_queue.Removes_the_Current_Target();
                Updates();
            }

            float remaining_angle() => Quaternion.Angle(r, tr);
            float remaining_distance() => p.DistanceTo(tp);
        }

        private void it_teleports_to(WMovementQueue.TargetLocation the_target)
        {
            its_speed = 0;
            its_angular_speed = 0;
            its_transform.parent = the_target.s_Parent;
            its_transform.localPosition = Vector3.zero;
            its_transform.localRotation = the_target.s_Rotation;
            
            the_queue.Removes_the_Current_Target();
        }

        private readonly WMovementQueue the_queue;
        private readonly MovementSettings its_settings;
        private readonly float its_boost_speed_multiplier;
        private readonly Transform its_transform;

        private readonly float its_acceleration;
        private readonly float its_angular_acceleration;

        private float its_speed;
        private float its_angular_speed;
        private bool it_is_fast_forwarding;
    }
}
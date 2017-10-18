using Lanski.Maths;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    public class WRotator
    {
        public WRotator(MovementSettings settings, WGameTime the_time, WSpacial the_spacial)
        {
            this.the_time = the_time;
            this.the_spacial = the_spacial;
            its_max_speed = settings.MaxAngularSpeed;
            its_min_speed = settings.MinAngularSpeed;
            its_acceleration_distance = settings.AnglularAccelerationDistance;
            
            its_movements = new Stream<TheVoid>();
            its_acceleration = MovementHelper.the_acceleration_from(its_max_speed, its_min_speed, its_acceleration_distance);
        }

        public Stream<TheVoid> s_Movements => its_movements;

        public bool Rotates_To(Direction2D the_target_orientation)
        {
            if (!the_spacial.s_Orientation.has_a_Value(out var the_current_orientation) || the_current_orientation != the_target_orientation)
            {
                the_spacial.s_Orientation = the_target_orientation;
            }
            
            var dt = the_time.s_Elapsed_Seconds;
            var r  = its_rotation();
            var to = the_target_orientation;
            var tr = to.s_Rotation();
            var dr = its_remaining_angle();

            var s = its_speed;
            var a = its_acceleration;
            var maxs = its_max_speed;
            var mins = its_min_speed;
            var maxdr = its_acceleration_distance;
                    
            var ts = its_target_speed();
            s = its_speed = its_new_rotation_speed();
            r = the_spacial.s_Local_Rotation = its_new_rotation();

            if (its_remaining_angle().is_Approximately(0f, 0.5f))
            {
                the_spacial.s_Local_Rotation = tr;
                return true;
            }

            return false;

            Quaternion its_rotation() => the_spacial.s_Local_Rotation;
            float its_target_speed() => dr > maxdr ? maxs : mins;
            float its_new_rotation_speed() => the_time.is_Fast_Forwarding ? maxs : Mathf.MoveTowards(s, ts, a * dt);
            Quaternion its_new_rotation() => Quaternion.RotateTowards(r, tr, s * dt);
            float its_remaining_angle() => Quaternion.Angle(r, tr);
        }
        
        public void Resets() => its_speed = 0;

        private readonly WGameTime the_time;
        private readonly WSpacial the_spacial;
        private readonly Stream<TheVoid> its_movements;

        private readonly float its_acceleration_distance;
        private readonly float its_acceleration;
        private readonly float its_max_speed;
        private readonly float its_min_speed;

        private float its_speed;
    }
}
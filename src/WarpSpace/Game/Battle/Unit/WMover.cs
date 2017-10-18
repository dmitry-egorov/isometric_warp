using Lanski.Geometry;
using Lanski.Maths;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    public class WMover
    {
        public WMover(MovementSettings settings, WGameTime the_time, WSpacial the_spacial)
        {
            this.the_time = the_time;
            this.the_spacial = the_spacial;
            its_max_speed = settings.MaxSpeed;
            its_min_speed = settings.MinSpeed;
            its_acceleration_distance = settings.AccelerationDistance;
            
            its_movements = new Stream<TheVoid>();
            its_acceleration = MovementHelper.the_acceleration_from(settings.MaxSpeed, settings.MinSpeed, settings.AccelerationDistance);
        }

        public Stream<TheVoid> s_Movements => its_movements;

        public bool Moves_To(Index2D the_target_position)
        {
            if (!the_spacial.s_Position.has_a_Value(out var the_current_position) || the_current_position != the_target_position)
            {
                the_spacial.s_Position = the_target_position;
            }
            
            var dt = the_time.s_Elapsed_Seconds;
            var p  = its_current_position();
            var tp = Vector3.zero;
            var dp = its_remaining_distance();
                    
            var s = this.its_speed;
            var a = its_acceleration;
            var maxs = its_max_speed;
            var mins = its_min_speed;
            var maxdp = its_acceleration_distance;

            var ts = its_target_speed();
            s = this.its_speed = its_speed();
            p = the_spacial.s_Local_Position = its_position();
            
            its_movements.Next();

            if (its_remaining_distance().is_Approximately(0f, 0.001f))
            {
                the_spacial.s_Local_Position = tp;
                return true;
            }

            return false;

            Vector3 its_current_position() => the_spacial.s_Local_Position; 
            float its_target_speed() => dp > maxdp ? maxs : mins; 
            float its_speed() => the_time.is_Fast_Forwarding ? maxs : Mathf.MoveTowards(s, ts, a * dt);
            Vector3 its_position() => Vector3.MoveTowards(p, tp, s * dt);
            float its_remaining_distance() => p.s_Distance_To(tp);
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
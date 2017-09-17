using System;
using Lanski.Geometry;
using Lanski.Maths;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    public class WExecutor
    {
        public WExecutor
        (
            WAgenda the_agenda, 
            MovementSettings the_settings, 
            float the_boost_speed_multiplier, 
            WSpacial the_spacial
        )
        {
            this.the_agenda = the_agenda;
            its_spacial = the_spacial;
            its_settings = the_settings;
            its_boost_time_multiplier = the_boost_speed_multiplier;
            its_movements = new Stream<TheVoid>();

            its_acceleration = the_acceleration_from(its_settings.MaxSpeed, its_settings.MinSpeed, its_settings.AccelerationDistance);
            its_angular_acceleration = the_acceleration_from(its_settings.MaxAngularSpeed, its_settings.MinAngularSpeed, its_settings.AnglularAccelerationDistance);

            float the_acceleration_from(float max_speed, float min_speed, float acceleration_distance)
            {
                var v0 = max_speed;
                var v1 = min_speed;
                var d = acceleration_distance;

                return Mathf.Abs((v1 * v1 - v0 * v0) / (2f * d));
            }
        }
        public IStream<TheVoid> s_Movements => its_movements;

        public void Fast_Forwards() => it_is_fast_forwarding = true;
        public void Resumes_Normal_Speed() => it_is_fast_forwarding = false;

        public void Updates() => it_updates();

        private float the_elapsed_time() => (it_is_fast_forwarding ? its_boost_time_multiplier : 1f) * Time.deltaTime;

        private void it_updates()
        {
            if (!the_agenda.s_Next_Task(out var the_task))
                return;

            var is_complete = it_executes_a_tasks_step(the_task.s_Task);
            
            if (is_complete)
                the_task.Completes();
        }

        private bool it_executes_a_tasks_step(WAgenda.DTask the_task)
        {
            if (the_task.is_to_Wait_For_a_Task(out var the_task_container))
                return it_waits_for(the_task_container);

            if (the_task.is_to_Hide())
                return it_teleports_to(Possible.Empty<Index2D>(), Direction2D.Left);

            if (the_task.is_to_Show_Up_At(out var the_position, out var the_orientation))
                return it_teleports_to(the_position, the_orientation);

            if (the_task.is_to_Move_To(out the_position))
                return it_moves_to(the_position);

            if (the_task.is_to_Rotate_To(out the_orientation))
                return it_rotates_to(the_orientation);

            throw new InvalidOperationException("Unknown task");
        }

        private bool it_waits_for(WAgenda.Task the_task) => the_task.is_Complete;

        private bool it_rotates_to(Direction2D the_target_orientation)
        {
            if (!its_spacial.s_Orientation.has_a_Value(out var the_current_orientation) || the_current_orientation != the_target_orientation)
            {
                its_spacial.s_Orientation = the_target_orientation;
            }
            
            var dt = the_elapsed_time();
            var r  = its_rotation();
            var to = the_target_orientation;
            var tr = to.s_Rotation();
            var dr = its_remaining_angle();

            var s = its_angular_speed;
            var a = its_angular_acceleration;
            var maxs = its_settings.MaxAngularSpeed;
            var mins = its_settings.MinAngularSpeed;
            var maxdr = its_settings.AnglularAccelerationDistance;
                    
            var ts = its_target_speed();
            s = its_angular_speed = its_new_rotation_speed();
            r = its_spacial.s_Local_Rotation = its_new_rotation();

            if (its_remaining_angle().is_Approximately(0f, 0.5f))
            {
                its_spacial.s_Local_Rotation = tr;
                return true;
            }

            return false;

            Quaternion its_rotation() => its_spacial.s_Local_Rotation;
            float its_target_speed() => dr > maxdr ? maxs : mins;
            float its_new_rotation_speed() => it_is_fast_forwarding ? maxs : Mathf.MoveTowards(s, ts, a * dt);
            Quaternion its_new_rotation() => Quaternion.RotateTowards(r, tr, s * dt);
            float its_remaining_angle() => Quaternion.Angle(r, tr);
        }

        private bool it_moves_to(Index2D the_target_position)
        {
            if (!its_spacial.s_Position.has_a_Value(out var the_current_position) || the_current_position != the_target_position)
            {
                its_spacial.s_Position = the_target_position;
            }
            
            var dt = the_elapsed_time();
            var p  = its_current_position();
            var tp = Vector3.zero;
            var dp = its_remaining_distance();
                    
            var s = this.its_speed;
            var a = its_acceleration;
            var maxs = its_settings.MaxSpeed;
            var mins = its_settings.MinSpeed;
            var maxdp = its_settings.AccelerationDistance;

            var ts = its_target_speed();
            s = this.its_speed = its_speed();
            p = its_spacial.s_Local_Position = its_position();
            
            its_movements.Next();

            if (its_remaining_distance().is_Approximately(0f, 0.001f))
            {
                its_spacial.s_Local_Position = tp;
                return true;
            }

            return false;

            Vector3 its_current_position() => its_spacial.s_Local_Position; 
            float its_target_speed() => dp > maxdp ? maxs : mins; 
            float its_speed() => it_is_fast_forwarding ? maxs : Mathf.MoveTowards(s, ts, a * dt);
            Vector3 its_position() => Vector3.MoveTowards(p, tp, s * dt);
            float its_remaining_distance() => p.s_Distance_To(tp);
        }

        private bool it_teleports_to(Possible<Index2D> the_position, Direction2D the_orientation)
        {
            its_speed = 0;
            its_angular_speed = 0;
            its_spacial.s_Position = the_position;
            its_spacial.s_Local_Position = Vector3.zero;
            its_spacial.s_Orientation = the_orientation;
            its_spacial.s_Local_Rotation = the_orientation.s_Rotation();

            its_movements.Next();

            return true;
        }

        private readonly WSpacial its_spacial;
        private readonly WAgenda the_agenda;
        private readonly MovementSettings its_settings;
        private readonly float its_boost_time_multiplier;

        private readonly float its_acceleration;
        private readonly float its_angular_acceleration;
        private readonly Stream<TheVoid> its_movements;
        private float its_speed;
        private float its_angular_speed;
        private bool it_is_fast_forwarding;
    }
}
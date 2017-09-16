using System;
using Lanski.Geometry;
using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    public class WMover
    {
        public WMover(WAgenda the_agenda, MovementSettings the_settings, float the_its_boost_speed_multiplier, Transform the_transform, Transform the_limbo)
        {
            this.the_agenda = the_agenda;
            its_settings = the_settings;
            its_transform = the_transform;
            this.the_limbo = the_limbo;
            its_boost_time_multiplier = the_its_boost_speed_multiplier;
            its_movements = new Stream<TheVoid>();

            its_acceleration = the_acceleration_from(its_settings.MaxSpeed, its_settings.MinSpeed, its_settings.AccelerationDistance);
            its_angular_acceleration = the_acceleration_from(its_settings.MaxAngularSpeed, its_settings.MinAngularSpeed, its_settings.AnglularAccelerationDistance);

            its_transform.localRotation = Direction2D.Left.s_Rotation();
            
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

            if (the_task.is_to_Hide())
            {
                it_teleports_to(the_limbo, Quaternion.identity);
            }
            else if (the_task.is_to_Show_At(out var the_parent, out var the_orientation))
            {
                it_teleports_to(the_parent, the_orientation.s_Rotation());
            }
            else if (the_task.is_to_Wait_For(out var the_task_container))
            {
                it_waits_for(the_task_container);
            }
            else if (the_task.is_to_Move_To(out the_parent))
            {
                it_moves_to(the_parent);
            }
            else if (the_task.is_to_Rotate_To(out the_orientation))
            {
                it_rotates_to(the_orientation.s_Rotation());
            }
            else
            {
                throw new InvalidOperationException("Unknown task");
            }

            its_movements.Next();
        }

        private void it_waits_for(WAgenda.TaskContainer the_task_container)
        {
            if (!the_task_container.is_Complete) 
                return;
            
            the_agenda.Completes_the_Current_Task();
        }

        private void it_rotates_to(Quaternion the_rotation)
        {
            var dt = the_elapsed_time();
            var r = its_transform.localRotation;
            var tr = the_rotation;
            var dr = remaining_angle();

            if (dr == 0f)
                return;

            var s = its_angular_speed;
            var a = its_angular_acceleration;
            var maxs = its_settings.MaxAngularSpeed;
            var mins = its_settings.MinAngularSpeed;
            var maxdr = its_settings.AnglularAccelerationDistance;
                    
            var ts = the_target_speed();
            s = its_angular_speed = new_rotation_speed();
            r = its_transform.localRotation = new_rotation();
            
            if (remaining_angle() == 0f)
                the_agenda.Completes_the_Current_Task();

            float the_target_speed() => dr > maxdr ? maxs : mins;
            float new_rotation_speed() => it_is_fast_forwarding ? maxs : Mathf.MoveTowards(s, ts, a * dt);
            Quaternion new_rotation() => Quaternion.RotateTowards(r, tr, s * dt);
            float remaining_angle() => Quaternion.Angle(r, tr);
        }

        private void it_moves_to(Transform the_parent)
        {
            it_updates_the_parent();
            
            var dt = the_elapsed_time();
            var p = its_transform.localPosition;
            var tp = Vector3.zero;
            var dp = remaining_distance();
                    
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
            
            if (remaining_distance() == 0f)
                the_agenda.Completes_the_Current_Task();

            float the_target_speed() => dp > maxdp ? maxs : mins; 
            float new_speed() => it_is_fast_forwarding ? maxs : Mathf.MoveTowards(s, ts, a * dt);
            Vector3 new_position() => Vector3.MoveTowards(p, tp, s * dt);
            float remaining_distance() => p.s_Distance_To(tp);

            void it_updates_the_parent()
            {
                if (its_transform.parent == the_parent) 
                    return;
                its_transform.parent = the_parent;
            }
        }

        private void it_teleports_to(Transform the_parent, Quaternion the_orientation)
        {
            its_speed = 0;
            its_angular_speed = 0;
            its_transform.parent = the_parent;
            its_transform.localPosition = Vector3.zero;
            its_transform.localRotation = the_orientation;
            
            the_agenda.Completes_the_Current_Task();
        }

        private readonly WAgenda the_agenda;
        private readonly MovementSettings its_settings;
        private readonly float its_boost_time_multiplier;
        private readonly Transform its_transform;
        private readonly Transform the_limbo;

        private readonly float its_acceleration;
        private readonly float its_angular_acceleration;
        private readonly Stream<TheVoid> its_movements;

        private float its_speed;
        private float its_angular_speed;
        private bool it_is_fast_forwarding;
    }
}
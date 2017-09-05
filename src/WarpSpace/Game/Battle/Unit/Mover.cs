using System.Collections.Generic;
using Lanski.Geometry;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Tile;

namespace WarpSpace.Game.Battle.Unit
{
    public class Mover
    {
        private readonly MovementSettings _settings;
        private readonly Transform _transform;
        private readonly float _acceleration;
        private readonly float _angularAcceleration;
            
        private readonly Queue<MovementTarget> _movementQueue = new Queue<MovementTarget>(16);

        private Possible<MovementTarget> _currentTarget;
        private float _speed;
        private float _angularSpeed;

        public Mover(MovementSettings settings, Transform transform)
        {
            _settings = settings;
            _transform = transform;

            _acceleration = CalculateAcceleration(_settings.MaxSpeed, _settings.MinSpeed, _settings.AccelerationDistance);
            _angularAcceleration = CalculateAcceleration(_settings.MaxAngularSpeed, _settings.MinAngularSpeed, _settings.AnglularAccelerationDistance);
                
            float CalculateAcceleration(float maxSpeed, float minSpeed, float accelerationDistance)
            {
                var v0 = maxSpeed;
                var v1 = minSpeed;
                var d = accelerationDistance;

                return Mathf.Abs((v1 * v1 - v0 * v0) / (2f * d));
            }
        }

        public void ScheduleMovement(TileComponent tile, Direction2D orientation)
        {
            var parent = tile.UnitSlot.transform;
            var rotation = orientation.To_Rotation();
            var target = new MovementTarget(parent, Vector3.zero, rotation);

            _movementQueue.Enqueue(target);
        }

        public void Update()
        {
            Try_to_Update_the_Target();
            if (!There_Is_a_Target(out var target))
                return;

            var parent = target.Parent;
            var dt = Time.deltaTime;
            var tr = target.Rotation;
            var tp = target.Position;
                
            Try_to_Update_the_Parent();
                
            var r = _transform.localRotation;
            var p = _transform.localPosition;

            Try_to_Update_objects_Rotation();
            Try_to_Update_objects_Position();
            Check_if_Target_Is_Reached_and_Repeat_a_Step_if_necessary();

            void Try_to_Update_the_Target()
            {
                while (Queue_Is_Not_Empty() && (there_Is_No_Target() || the_Current_Target_Has_the_Same_Rotation_As_the_Next_Target_in_the_queue()))
                    Update_Target_From_the_Queue();

                bool Queue_Is_Not_Empty() => _movementQueue.Count != 0;
                bool the_Current_Target_Has_the_Same_Rotation_As_the_Next_Target_in_the_queue() => 
                    There_Is_a_Target(out var current_target) 
                    && current_target.Rotation == _movementQueue.Peek().Rotation;
                    
                void Update_Target_From_the_Queue() => _currentTarget = _movementQueue.Dequeue().As_a_Slot();
                bool there_Is_No_Target() => _currentTarget.Has_Nothing();
            }

                
            void Try_to_Update_the_Parent()
            {
                if (_transform.parent == parent) 
                    return;
                
                _transform.parent = parent;
            }

            void Try_to_Update_objects_Rotation()
            {
                var dr = Calc_remaining_angle();

                if (dr == 0f)
                    return;

                var s = _angularSpeed;
                var a = _angularAcceleration;
                var maxs = _settings.MaxAngularSpeed;
                var mins = _settings.MinAngularSpeed;
                var maxdr = _settings.AnglularAccelerationDistance;
                    
                var ts = Get_target_speed(); 
                _angularSpeed = s = Calculate_current_rotation_speed();
                _transform.localRotation = r = Calculate_current_rotation();

                float Get_target_speed() => dr > maxdr ? maxs : mins;
                float Calculate_current_rotation_speed() => Mathf.MoveTowards(s, ts, a * dt);
                Quaternion Calculate_current_rotation() => Quaternion.RotateTowards(r, tr, s * dt);
            }

            void Try_to_Update_objects_Position()
            {
                var dr = Calc_remaining_angle();
                var dp = Calc_remaining_distance();
                    
                if (dr != 0.0f) 
                    return;
                if (dp == 0.0f) 
                    return;
                    
                var s = _speed;
                var a = _acceleration;
                var maxs = _settings.MaxSpeed;
                var mins = _settings.MinSpeed;
                var maxdp = _settings.AccelerationDistance;

                var ts = Get_target_speed();
                _speed = s = Calculate_current_speed();
                _transform.localPosition = p = Calculate_current_position();

                float Get_target_speed() => dp > maxdp ? maxs : mins; 
                float Calculate_current_speed() => Mathf.MoveTowards(s, ts, a * dt);
                Vector3 Calculate_current_position() => Vector3.MoveTowards(p, tp, s * dt);
            }

            void Check_if_Target_Is_Reached_and_Repeat_a_Step_if_necessary()
            {
                var dr = Calc_remaining_angle();
                var dp = Calc_remaining_distance();
                    
                if (dp != 0f) 
                    return;
                if (dr != 0f) 
                    return;
                    
                _currentTarget = Possible.Empty<MovementTarget>();
                Update();
            }

            float Calc_remaining_angle() => Quaternion.Angle(r, tr);
            float Calc_remaining_distance() => p.DistanceTo(tp);
        }
        
        //Compiler error when making it a local function (since it's used on two different layers)
        private bool There_Is_a_Target(out MovementTarget current_target) => _currentTarget.Has_a_Value(out current_target);

        private struct MovementTarget
        {
            public readonly Transform Parent; 
            public readonly Vector3 Position;
            public readonly Quaternion Rotation;

            public MovementTarget(Transform parent, Vector3 position, Quaternion rotation)
            {
                Parent = parent;
                Position = position;
                Rotation = rotation;
            }
        }
    }
}
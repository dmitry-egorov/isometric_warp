using System;
using System.Collections.Generic;
using Lanski.Geometry;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;

namespace WarpSpace.Unity.World.Battle.Unit
{
    public class Component: MonoBehaviour
    {
        public GameObject Outline;
        public MovementSettings Movement;
        
        private Mover _mover;

        public static Component Create(GameObject prefab, Quaternion rotation, Transform parent)
        {
            var obj = Instantiate(prefab, parent).GetComponent<Component>();

            obj.Init(rotation);

            return obj;
        }

        private void Init(Quaternion rotation)
        {
            transform.localRotation = rotation;
            _mover = new Mover(Movement, transform);
        }

        public void Update()
        {
            _mover.Update();
        }

        public void EnableOutline()
        {
            Outline.SetActive(true);
        }

        public void DisableOutline()
        {
            Outline.SetActive(false);
        }

        public void MoveTo(Board.Tile.Component tile, Direction2D newRotation)
        {
            _mover.ScheduleMovement(tile, newRotation);
        }
        
        [Serializable]
        public struct MovementSettings
        {
            public float MaxAngularSpeed;
            public float MinAngularSpeed;
            public float AnglularAccelerationDistance;
            public float MaxSpeed;
            public float MinSpeed;
            public float AccelerationDistance;
        }

        private class Mover
        {
            private readonly MovementSettings _settings;
            private readonly Transform _transform;
            private readonly float _acceleration;
            private readonly float _angularAcceleration;
            
            private readonly Queue<MovementTarget> _movementQueue = new Queue<MovementTarget>(16);

            private MovementTarget? _currentTarget;
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

            public void ScheduleMovement(Board.Tile.Component tile, Direction2D orientation)
            {
                var parent = tile.UnitSlot.transform;
                var rotation = orientation.ToRotation();
                var target = new MovementTarget(parent, Vector3.zero, rotation);

                _movementQueue.Enqueue(target);
            }

            public void Update()
            {
                Try_to_Update_the_Target();
                if (Theres_No_Target())
                    return;

                var target = _currentTarget.Value;
                var parent = target.Parent;
                var dt = Time.deltaTime;
                var tr = target.Rotation;
                var tp = target.Position;
                
                Try_to_Update_objects_Parent();
                
                var r = _transform.localRotation;
                var p = _transform.localPosition;

                Try_to_Update_objects_Rotation();
                Try_to_Update_objects_Position();
                Check_if_Target_is_Reached_and_Repeat_a_Step_if_necessary();

                void Try_to_Update_the_Target()
                {
                    while (Queue_is_not_empty() && (Theres_No_Target() || Current_target_has_the_same_rotation_as_the_next_target_in_the_queue()))
                        Update_target_from_the_queue();

                    bool Queue_is_not_empty() => _movementQueue.Count != 0;
                    bool Current_target_has_the_same_rotation_as_the_next_target_in_the_queue() => _currentTarget.Value.Rotation == _movementQueue.Peek().Rotation;
                    void Update_target_from_the_queue()
                    {
                        _currentTarget = _movementQueue.Dequeue();
                    }
                }
                
                bool Theres_No_Target() => !_currentTarget.HasValue;

                void Try_to_Update_objects_Parent()
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

                void Check_if_Target_is_Reached_and_Repeat_a_Step_if_necessary()
                {
                    var dr = Calc_remaining_angle();
                    var dp = Calc_remaining_distance();
                    
                    if (dp != 0f) 
                        return;
                    if (dr != 0f) 
                        return;
                    
                    _currentTarget = null;
                    Update();
                }

                float Calc_remaining_angle() => Quaternion.Angle(r, tr);
                float Calc_remaining_distance() => p.DistanceTo(tp);
            }

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
}
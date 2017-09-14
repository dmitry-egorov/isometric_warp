using System;

namespace WarpSpace.Settings
{
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
}
namespace Core.Structures
{
    using System;
    using UnityEngine;

    public struct Vector3Wrapper : IEquatable<Vector3Wrapper>
    {
        public Vector3 Value { get; }

        public Vector3Wrapper(Vector3 value)
        {
            Value = value;
        }

        public bool Equals(Vector3Wrapper other)
        {
            var self = Value;
            var otherValue = other.Value;
            return self == otherValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Vector3Wrapper && Equals((Vector3Wrapper) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator Vector3Wrapper(Vector3 vector3)
        {
            return new Vector3Wrapper(vector3);
        }

        public static implicit operator Vector3(Vector3Wrapper wrapper)
        {
            return wrapper.Value;
        }
    }
}
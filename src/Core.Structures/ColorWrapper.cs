namespace Core.Structures
{
    using System;
    using UnityEngine;

    public struct ColorWrapper : IEquatable<ColorWrapper>
    {
        public Color Value { get; }

        public ColorWrapper(Color value)
        {
            Value = value;
        }

        public bool Equals(ColorWrapper other)
        {
            var selfColor = Value;
            var otherColor = other.Value;
            return 
                selfColor.a == otherColor.a 
                && selfColor.r == otherColor.r
                && selfColor.g == otherColor.g
                && selfColor.b == otherColor.b
                ;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ColorWrapper && Equals((ColorWrapper) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator ColorWrapper(Color color)
        {
            return new ColorWrapper(color);
        }

        public static implicit operator Color(ColorWrapper wrapper)
        {
            return wrapper.Value;
        }
    }
}
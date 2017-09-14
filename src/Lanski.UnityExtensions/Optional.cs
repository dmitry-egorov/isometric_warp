using System;
using System.Collections.Generic;
using Lanski.Structures;

namespace Lanski.UnityExtensions
{
    [Serializable]
    public abstract class Optional<T>
    {
        public bool Enabled;
        public T Value;

        public Possible<T> s_Possible => Enabled ? Value : Possible.Empty<T>();

        protected bool Equals(Optional<T> other)
        {
            return Enabled == other.Enabled && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Optional<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Enabled.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(Value);
            }
        }
    }
}
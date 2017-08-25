using System;

namespace Lanski.UnityExtensions
{
    [Serializable]
    public abstract class Optional<T>
        where T : struct
    {
        public bool Enabled;
        public T Value;

        public T? Nullable => Enabled ? Value : default(T?);

        protected bool Equals(Optional<T> other)
        {
            return Enabled == other.Enabled && Value.Equals(other.Value);
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
                return (Enabled.GetHashCode() * 397) ^ Value.GetHashCode();
            }
        }
    }
}
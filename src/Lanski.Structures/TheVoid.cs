using System;

namespace Lanski.Structures
{
    public struct TheVoid: IEquatable<TheVoid>
    {
        public static TheVoid Instance => new TheVoid();

        public bool Equals(TheVoid other) => true;
        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && (obj is TheVoid && Equals((TheVoid) obj));
        public override int GetHashCode() => 0;
    }
}
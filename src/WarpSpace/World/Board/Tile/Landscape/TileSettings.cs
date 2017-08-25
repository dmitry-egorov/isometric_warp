using System;
using UnityEngine;

namespace WarpSpace.World.Board.Tile.Landscape
{
    [Serializable]
    public struct TileSettings
    {
        public float OwnHeight;
        public float SameTypeHeight;
        public float SameTypeHeightCross;
        public float Falloff;
        public Mesh[] Meshes;

        public bool Equals(TileSettings other)
        {
            return OwnHeight.Equals(other.OwnHeight) && SameTypeHeight.Equals(other.SameTypeHeight) && SameTypeHeightCross.Equals(other.SameTypeHeightCross) && Falloff.Equals(other.Falloff) && Equals(Meshes, other.Meshes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TileSettings && Equals((TileSettings) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = OwnHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ SameTypeHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ SameTypeHeightCross.GetHashCode();
                hashCode = (hashCode * 397) ^ Falloff.GetHashCode();
                hashCode = (hashCode * 397) ^ (Meshes != null ? Meshes.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
using System;
using UnityEngine;

namespace WarpSpace.World.Board.Tile.Water
{
    [Serializable]
    public struct Settings
    {
        public Mesh[] Meshes;

        public bool Equals(Settings other)
        {
            return Equals(Meshes, other.Meshes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Settings && Equals((Settings) obj);
        }

        public override int GetHashCode()
        {
            return (Meshes != null ? Meshes.GetHashCode() : 0);
        }
    }
}
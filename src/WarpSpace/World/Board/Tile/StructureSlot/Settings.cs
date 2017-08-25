using System;
using UnityEngine;

namespace WarpSpace.World.Board.Tile.StructureSlot
{
    [Serializable]
    public struct Settings
    {
        public GameObject EntrancePrefab;

        public bool Equals(Settings other)
        {
            return Equals(EntrancePrefab, other.EntrancePrefab);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Settings && Equals((Settings) obj);
        }

        public override int GetHashCode()
        {
            return (EntrancePrefab != null ? EntrancePrefab.GetHashCode() : 0);
        }
    }
}
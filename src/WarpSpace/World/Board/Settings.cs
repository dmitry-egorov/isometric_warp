using System;
using System.Linq;
using Lanski.Linq;
using Lanski.UnityExtensions;
using UnityEngine;

namespace WarpSpace.World.Board
{
    [Serializable]
    public struct Settings
    {
        public GameObject TilePrefab;
        public GameObject MothershipPrefab;
        
        public Tile.Landscape.Settings Landscape;
        public Tile.StructureSlot.Settings Structure;
        public Tile.Water.Settings Water;

        public bool Equals(Settings other) => Equals(TilePrefab, other.TilePrefab) && Equals(MothershipPrefab, other.MothershipPrefab) && Landscape.Equals(other.Landscape) && Structure.Equals(other.Structure) && Water.Equals(other.Water);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Settings && Equals((Settings) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (TilePrefab != null ? TilePrefab.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MothershipPrefab != null ? MothershipPrefab.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Landscape.GetHashCode();
                hashCode = (hashCode * 397) ^ Structure.GetHashCode();
                hashCode = (hashCode * 397) ^ Water.GetHashCode();
                return hashCode;
            }
        }
    }
}
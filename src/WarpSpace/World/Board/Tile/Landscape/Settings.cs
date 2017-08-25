using System;

namespace WarpSpace.World.Board.Tile.Landscape
{
    [Serializable]
    public struct Settings
    {
        public TileSettings Mountain;
        public TileSettings Hill;
        public TileSettings Flatland;
        public TileSettings Water;

        public bool Equals(Settings other) => Mountain.Equals(other.Mountain) && Hill.Equals(other.Hill) && Flatland.Equals(other.Flatland) && Water.Equals(other.Water);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Settings && Equals((Settings) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Mountain.GetHashCode();
                hashCode = (hashCode * 397) ^ Hill.GetHashCode();
                hashCode = (hashCode * 397) ^ Flatland.GetHashCode();
                hashCode = (hashCode * 397) ^ Water.GetHashCode();
                return hashCode;
            }
        }
    }
}
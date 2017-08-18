using WarpSpace.Planet.Tiles;

namespace WarpSpace.World.Board
{
    public struct Tile
    {
        public readonly LandscapeType Type;

        public Tile(LandscapeType type)
        {
            Type = type;
        }

        public bool Equals(Tile other)
        {
            return Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Tile && Equals((Tile) obj);
        }

        public override int GetHashCode()
        {
            return (int) Type;
        }
    }
}
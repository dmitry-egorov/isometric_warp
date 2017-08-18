namespace WarpSpace.World.Board
{
    public struct TileSpec
    {
        public readonly LandscapeType Type;

        public TileSpec(LandscapeType type)
        {
            Type = type;
        }

        public bool Equals(TileSpec other)
        {
            return Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TileSpec && Equals((TileSpec) obj);
        }

        public override int GetHashCode()
        {
            return (int) Type;
        }
    }
}
namespace WarpSpace.Descriptions
{
    public struct TileDescription
    {
        public readonly LandscapeType Type;
        public readonly TileContentDescription Initial_Content;

        public TileDescription(LandscapeType type, TileContentDescription initial_content)
        {
            Type = type;
            Initial_Content = initial_content;
        }

        public bool Equals(TileDescription other)
        {
            return Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TileDescription && Equals((TileDescription) obj);
        }

        public override int GetHashCode()
        {
            return (int) Type;
        }
    }
}
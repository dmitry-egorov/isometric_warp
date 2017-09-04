namespace WarpSpace.Descriptions
{
    public struct TileDescription
    {
        public readonly LandscapeType Type;
        public readonly StructureDescription? Initial_Structure;

        public TileDescription(LandscapeType type, StructureDescription? initial_structure)
        {
            Type = type;
            Initial_Structure = initial_structure;
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
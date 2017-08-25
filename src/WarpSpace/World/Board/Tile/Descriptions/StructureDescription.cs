using Lanski.Structures;

namespace WarpSpace.World.Board.Tile.Descriptions
{
    public struct StructureDescription
    {
        public readonly StructureType Type;
        public readonly Direction2D Orientation;

        public StructureDescription(StructureType type, Direction2D orientation)
        {
            Type = type;
            Orientation = orientation;
        }
    }
}
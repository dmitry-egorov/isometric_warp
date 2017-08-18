using Lanski.Structures;

namespace WarpSpace.World.Board
{
    public struct StructureSpec
    {
        public readonly Index2D Index;
        public readonly Direction2D Direction;
        public readonly StructureType Type;

        public StructureSpec(Index2D index, Direction2D direction, StructureType type)
        {
            Index = index;
            Direction = direction;
            Type = type;
        }
    }
}
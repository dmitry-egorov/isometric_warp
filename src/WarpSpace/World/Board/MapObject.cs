using Lanski.Structures;

namespace WarpSpace.World.Board
{
    public struct MapObject
    {
        public readonly Index2D Index;
        public readonly Direction2D Direction;
        public readonly MapObjectType Type;

        public MapObject(Index2D index, Direction2D direction, MapObjectType type)
        {
            Index = index;
            Direction = direction;
            Type = type;
        }
    }
}
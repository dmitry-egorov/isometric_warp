using Lanski.Structures;

namespace WarpSpace.Descriptions
{
    public static class TileHelper
    {
        public static Direction2D GetOrientation(Index2D i)
        {
            return (Direction2D)((i.Column + i.Row) % 4);
        }
    }
}
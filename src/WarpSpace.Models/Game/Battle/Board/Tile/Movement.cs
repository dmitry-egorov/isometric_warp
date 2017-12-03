using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct Movement
    {
        public readonly MTile s_Source;
        public readonly MTile s_Destination;

        public Movement(MTile source, MTile destination)
        {
            s_Source = source;
            s_Destination = destination;
        }
    }
}
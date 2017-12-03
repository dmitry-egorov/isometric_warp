using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct Movement
    {
        public readonly MLocation s_Source;
        public readonly MLocation s_Destination;

        public Movement(MLocation source, MLocation destination)
        {
            s_Source = source;
            s_Destination = destination;
        }
    }
}
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct Movement
    {
        public readonly MUnitLocation s_Source;
        public readonly MUnitLocation s_Destination;

        public Movement(MUnitLocation source, MUnitLocation destination)
        {
            s_Source = source;
            s_Destination = destination;
        }
    }
}
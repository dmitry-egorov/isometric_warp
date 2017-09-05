using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct UnitMoved
    {
        public readonly MUnit Unit;
        public readonly MLocation Source;
        public readonly MLocation Destination;

        public UnitMoved(MUnit unit, MLocation source, MLocation destination)
        {
            Unit = unit;
            Source = source;
            Destination = destination;
        }
    }
}
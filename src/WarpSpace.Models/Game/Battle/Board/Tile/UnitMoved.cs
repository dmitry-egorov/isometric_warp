using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct UnitMoved
    {
        public readonly UnitModel Unit;
        public readonly LocationModel Source;
        public readonly LocationModel Destination;

        public UnitMoved(UnitModel unit, LocationModel source, LocationModel destination)
        {
            Unit = unit;
            Source = source;
            Destination = destination;
        }
    }
}
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public struct UnitCreated
    {
        public readonly MUnit Unit;
        public readonly MLocation Initial_Location;

        public UnitCreated(MUnit unit, MLocation initial_location)
        {
            Unit = unit;
            Initial_Location = initial_location;
        }
    }
}
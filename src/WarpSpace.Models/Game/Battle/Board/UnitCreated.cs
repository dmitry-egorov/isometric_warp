using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public struct UnitCreated
    {
        public readonly UnitModel Unit;
        public readonly LocationModel Initial_Location;

        public UnitCreated(UnitModel unit, LocationModel initial_location)
        {
            Unit = unit;
            Initial_Location = initial_location;
        }
    }
}
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public struct UnitDestroyed
    {
        public readonly UnitModel Unit;
        public readonly LocationModel Last_Location;

        public UnitDestroyed(UnitModel unit, LocationModel last_location)
        {
            Unit = unit;
            Last_Location = last_location;
        }
    }
}
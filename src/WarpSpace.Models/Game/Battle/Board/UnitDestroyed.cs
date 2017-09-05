using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public struct UnitDestroyed
    {
        public readonly MUnit Unit;
        public readonly MLocation Last_Location;

        public UnitDestroyed(MUnit unit, MLocation last_location)
        {
            Unit = unit;
            Last_Location = last_location;
        }
    }
}
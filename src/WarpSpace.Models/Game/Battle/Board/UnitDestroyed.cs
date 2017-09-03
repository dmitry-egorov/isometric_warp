using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public struct UnitDestroyed
    {
        public readonly UnitModel Unit;
        public readonly TileModel Location;

        public UnitDestroyed(UnitModel unit, TileModel location)
        {
            Unit = unit;
            Location = location;
        }
    }
}
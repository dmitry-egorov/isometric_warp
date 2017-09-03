using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public struct UnitAdded
    {
        public readonly UnitModel Unit;
        public readonly TileModel SourceTile;

        public UnitAdded(UnitModel unit, TileModel sourceTile)
        {
            Unit = unit;
            SourceTile = sourceTile;
        }
    }
}
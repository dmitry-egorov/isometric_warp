using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct Movement
    {
        public readonly MUnitLocation Source;
        public readonly MUnitLocation Destination;

        public Movement(MUnitLocation source, MUnitLocation destination)
        {
            Source = source;
            Destination = destination;
        }
    }
}
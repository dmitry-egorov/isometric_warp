using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct Movement
    {
        public readonly MLocation Source;
        public readonly MLocation Destination;

        public Movement(MLocation source, MLocation destination)
        {
            Source = source;
            Destination = destination;
        }
    }
}
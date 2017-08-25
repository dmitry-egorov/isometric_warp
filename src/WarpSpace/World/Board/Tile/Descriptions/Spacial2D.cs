using Lanski.Structures;

namespace WarpSpace.World.Board.Tile.Descriptions
{
    public struct Spacial2D
    {
        public readonly Index2D Position;
        public readonly Direction2D Orientation;

        public Spacial2D(Index2D position, Direction2D orientation)
        {
            Position = position;
            Orientation = orientation;
        }

        public bool Equals(Spacial2D other) => Position.Equals(other.Position) && Orientation == other.Orientation;
        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && (obj is Spacial2D && Equals((Spacial2D) obj));
        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() * 397) ^ (int) Orientation;
            }
        }
    }
}
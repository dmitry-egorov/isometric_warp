using System.Linq;
using Lanski.Structures;

namespace WarpSpace.Descriptions
{
    public struct BoardDescription
    {
        public readonly TileDescription[,] Tiles;
        public readonly Spacial2D EntranceSpacial;

        public BoardDescription(TileDescription[,] tiles, Spacial2D entranceSpacial)
        {
            Tiles = tiles;
            EntranceSpacial = entranceSpacial;
        }

        public bool Equals(BoardDescription other)
        {
            return Equals(Tiles, other.Tiles) && EntranceSpacial.Equals(other.EntranceSpacial);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BoardDescription && Equals((BoardDescription) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Tiles.GetHashCode() * 397) ^ EntranceSpacial.GetHashCode();
            }
        }

        public string Display()
        {
            var rows = Tiles
                .EnumerateRows()
                .Select(row => string.Join(" ", row
                    .Select(t => t.Type.ToChar().ToString())
                    .ToArray()))
                .ToArray();
            
            return string.Join("\n", rows);
        }
    }
}
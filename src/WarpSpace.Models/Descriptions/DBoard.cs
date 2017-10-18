using System.Linq;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public struct DBoard
    {
        public readonly DTile[,] Tiles;
        public readonly Spacial2D EntranceSpacial;

        public DBoard(DTile[,] tiles, Spacial2D entranceSpacial)
        {
            Tiles = tiles;
            EntranceSpacial = entranceSpacial;
        }

        public bool Equals(DBoard other)
        {
            return Equals(Tiles, other.Tiles) && EntranceSpacial.Equals(other.EntranceSpacial);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DBoard && Equals((DBoard) obj);
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
                    .Select(t => t.s_Type.s_Serialization_Symbol.ToString())
                    .ToArray()))
                .ToArray();
            
            return string.Join("\n", rows);
        }
    }
}
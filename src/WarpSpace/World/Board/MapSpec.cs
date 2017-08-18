using System.Linq;
using Lanski.Structures;

namespace WarpSpace.World.Board
{
    public struct MapSpec
    {
        public readonly TileSpec[,] Tiles;
        public readonly StructureSpec[] Structures;

        public MapSpec(TileSpec[,] tiles, StructureSpec[] structures)
        {
            Tiles = tiles;
            Structures = structures;
        }

        public bool Equals(MapSpec other)
        {
            return Equals(Tiles, other.Tiles) && Equals(Structures, other.Structures);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MapSpec && Equals((MapSpec) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Tiles != null ? Tiles.GetHashCode() : 0) * 397) ^ (Structures != null ? Structures.GetHashCode() : 0);
            }
        }

        public override string ToString()
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
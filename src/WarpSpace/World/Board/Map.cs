using WarpSpace.Planet.Tiles;

namespace WarpSpace.World.Board
{
    public struct Map
    {
        public readonly Tile[,] Tiles;
        public readonly MapObject[] MapObjects;

        public Map(Tile[,] tiles, MapObject[] mapObjects)
        {
            Tiles = tiles;
            MapObjects = mapObjects;
        }

        public bool Equals(Map other)
        {
            return Equals(Tiles, other.Tiles) && Equals(MapObjects, other.MapObjects);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Map && Equals((Map) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Tiles != null ? Tiles.GetHashCode() : 0) * 397) ^ (MapObjects != null ? MapObjects.GetHashCode() : 0);
            }
        }
    }
}
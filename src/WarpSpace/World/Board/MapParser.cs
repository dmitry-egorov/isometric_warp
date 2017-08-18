using System.Linq;
using Lanski.Structures;

namespace WarpSpace.World.Board
{
    public static class MapParser
    {
        public static MapSpec ParseMap(Factory.MapData map)
        {
            var split = map.Tiles.Split('\n')
                .Select(row => row.Split(' ')
                    .Select(s => s[0])
                    .Select(ParseChar)
                    .Select(t => new TileSpec(t)));

            var tiles = split.To2DArray();

            var structures = new[] {new StructureSpec(map.EntrancePosition.ToIndex2D(), map.EntranceDirection, StructureType.Entrance)};
            return new MapSpec(tiles, structures);

            LandscapeType ParseChar(char c)
            {
                return c.ToLandscapeType();
            }
        }
    }
}
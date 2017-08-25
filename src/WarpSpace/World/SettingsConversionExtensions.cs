using System.Linq;
using Lanski.Structures;
using WarpSpace.Common;
using WarpSpace.World.Board.Tile.Descriptions;

namespace WarpSpace.World
{
    public static class SettingsConversionExtensions
    {
        public static BoardDescription ToSpec(this Factory.BoardData boardData)
        {
            var entrance = ToSpacial2D(boardData.Entrance);
            var entrancePosition = entrance.Position;
            var entranceOrientation = entrance.Orientation;
    
            var tiles = boardData.Tiles.Split('\n')
                    .Select(row => row.Split(' ')
                        .Select(s => s[0])
                        .Select(ParseChar))
                    .To2DArray()
                    .Map(CreateTile)
                ;
    
    
            return new BoardDescription(tiles, entrance);
    
            LandscapeType ParseChar(char c)
            {
                return c.ToLandscapeType();
            }
                
            TileDescription CreateTile(LandscapeType t, Index2D i)
            {
                return new TileDescription(t,
                    i == entrancePosition
                        ? (StructureDescription?)new StructureDescription(StructureType.Entrance, entranceOrientation)
                        : null);
            }
        }

        public static Spacial2D ToSpacial2D(Spacial2DData data) => new Spacial2D(data.Position.ToIndex2D(), data.Orientation);
    }
}
using System.Linq;
using Lanski.Structures;
using WarpSpace.Common;
using WarpSpace.Common.MapParsing;
using WarpSpace.Descriptions;

namespace WarpSpace.Unity.World.Battle
{
    public static class SettingsConversionExtensions
    {
        public static BoardDescription ToDescription(this Component.BoardData boardData)
        {
            var entrance = ToSpacial2D(boardData.Entrance);
            var exit = ToSpacial2D(boardData.Exit);

            var tiles = boardData
                    .Tiles
                    .ToArray2D()
                    .Map(ParseLandscapeChar)
                    .Map(CreateTile)
                ;

            var units = boardData
                .Units
                .ToArray2D()
                .Map(ParseUnitChar)
                .Map(CreateUnitDescritpion)
                ;
    
            return new BoardDescription(tiles, entrance, units);
    
            LandscapeType ParseLandscapeChar(char c) => c.ToLandscapeType();

            UnitType? ParseUnitChar(char c) => c.ToUnitType();
                
            TileDescription CreateTile(LandscapeType t, Index2D i) => 
                new TileDescription(t,
                SelectStructure(i));

            UnitDescription? CreateUnitDescritpion(UnitType? arg) => arg.Select(t => new UnitDescription(t)); 
            
            StructureDescription? SelectStructure(Index2D i)
            {
                if (i == entrance.Position)
                    return new StructureDescription(StructureType.Entrance, entrance.Orientation);
                if (i == exit.Position)
                    return new StructureDescription(StructureType.Exit, exit.Orientation);
                
                return null;
            }
            
            Spacial2D ToSpacial2D(Spacial2DData data) => new Spacial2D(data.Position.ToIndex2D(), data.Orientation);
        }
    }
}
using System.Collections.Generic;
using Lanski.Structures;
using WarpSpace.Common;
using WarpSpace.Common.MapParsing;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Game.Battle
{
    public static class SettingsConversionExtensions
    {
        public static BoardDescription ToDescription(this BoardData boardData)
        {
            var entrance = ToSpacial2D(boardData.Entrance);
            var exit = ToSpacial2D(boardData.Exit);

            var units = boardData
                    .Units
                    .ToArray2D()
                    .Map(ParseUnitChar)
                    .Map(CreateUnitDescritpion)
                ;
            
            var tiles = boardData
                    .Tiles
                    .ToArray2D()
                    .Map(ParseLandscapeChar)
                    .Map(CreateTile)
                ;

            return new BoardDescription(tiles, entrance);
    
            LandscapeType ParseLandscapeChar(char c) => c.ToLandscapeType();

            Possible<UnitType> ParseUnitChar(char c) => c.ToUnitType();
                
            TileDescription CreateTile(LandscapeType t, Index2D i) => 
                new TileDescription(t, SelectContent(i));

            //TODO: generate random loot from settings?
            Possible<UnitDescription> CreateUnitDescritpion(Possible<UnitType> arg) => 
                arg.Select(type => new UnitDescription(type, Faction.Natives, Stuff.Initial_For(type), Possible.Empty<IReadOnlyList<Possible<UnitDescription>>>()))
            ; 
            
            TileSiteDescription SelectContent(Index2D i)
            {
                if (i == entrance.Position)
                    return StructureDescription.Create.Entrance(entrance.Orientation);
                if (i == exit.Position)
                    return StructureDescription.Create.Exit(exit.Orientation);

                return units.Get(i).has_a_Value(out var unit) 
                    ? (TileSiteDescription) unit 
                    : TheVoid.Instance;
            }
            
            Spacial2D ToSpacial2D(Spacial2DData data) => new Spacial2D(data.Position.ToIndex2D(), data.Orientation);
        }
    }
}
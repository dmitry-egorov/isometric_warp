using System.Collections.Generic;
using Lanski.Structures;
using WarpSpace.Common;
using WarpSpace.Common.MapParsing;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Game.Battle
{
    public static class SettingsConversionExtensions
    {
        public static DBoard ToDescription(this BoardData boardData)
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

            return new DBoard(tiles, entrance);
    
            LandscapeType ParseLandscapeChar(char c) => c.s_Landscape_Type();

            Possible<UnitType> ParseUnitChar(char c) => c.s_Unit_Type();
                
            DTile CreateTile(LandscapeType t, Index2D i) => 
                new DTile(t, SelectContent(i));

            //TODO: generate random loot from settings?
            Possible<DUnit> CreateUnitDescritpion(Possible<UnitType> arg) => 
                arg.Select(type => new DUnit(type, Faction.the_Natives, type.s_Initial_Staff(), Possible.Empty<IReadOnlyList<Possible<DUnit>>>()))
            ; 
            
            DTileSite SelectContent(Index2D i)
            {
                if (i == entrance.Position)
                    return DStructure.Create.Entrance(entrance.Orientation);
                if (i == exit.Position)
                    return DStructure.Create.Exit(exit.Orientation);

                return units.Get(i).has_a_Value(out var unit) 
                    ? (DTileSite) unit 
                    : TheVoid.Instance;
            }
            
            Spacial2D ToSpacial2D(Spacial2DData data) => new Spacial2D(data.Position.ToIndex2D(), data.Orientation);
        }
    }
}
using Lanski.Structures;
using WarpSpace.Common;
using WarpSpace.Common.MapParsing;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board;

namespace WarpSpace.Game.Battle
{
    public static class SettingsConversionExtensions
    {
        public static BoardDescription ToDescription(this BattleComponent.BoardData boardData)
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

            Slot<UnitType> ParseUnitChar(char c) => c.ToUnitType();
                
            TileDescription CreateTile(LandscapeType t, Index2D i) => 
                new TileDescription(t, SelectContent(i));

            //TODO: generate random loot from settings?
            Slot<UnitDescription> CreateUnitDescritpion(Slot<UnitType> arg) => //TODO: match bay size
                arg.Select(type => new UnitDescription(type, Faction.Natives, InventoryContent.Initial_For(type), Slot.Empty<Slot<UnitDescription>[]>()))
            ; 
            
            TileContentDescription SelectContent(Index2D i)
            {
                if (i == entrance.Position)
                    return StructureDescription.Create.Entrance(entrance.Orientation);
                if (i == exit.Position)
                    return StructureDescription.Create.Exit(exit.Orientation);

                return units.Get(i).Has_a_Value(out var unit) 
                    ? (TileContentDescription) unit 
                    : TheVoid.Instance;
            }
            
            Spacial2D ToSpacial2D(Spacial2DData data) => new Spacial2D(data.Position.ToIndex2D(), data.Orientation);
        }
    }
}
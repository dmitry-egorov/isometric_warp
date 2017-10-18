using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Common.MapParsing;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.BoardGenerators
{
    [CreateAssetMenu(fileName = "Board", menuName = "Custom/Boards/Predefined", order = 1)]
    public class PredefinedBoard : BoardSettings
    {
        [TextArea(8,8)] public string Tiles;
        [TextArea(8,8)] public string Units;
        public Spacial2DData Entrance;
        public Spacial2DData Exit;

        public override DBoard s_Description_With(IReadOnlyList<MUnitType> the_unit_types, IReadOnlyList<MLandscapeType> the_landscape_types, MFaction the_native_faction, MChassisType the_mothership_chassis_type)
        {
            var the_unit_types_map = the_unit_types.ToDictionary(ut => ut.s_Serialization_Symbol);
            var the_landscape_types_map = the_landscape_types.ToDictionary(ut => ut.s_Serialization_Symbol);
            var entrance = ToSpacial2D(Entrance);
            var exit = ToSpacial2D(Exit);

            var units = 
                    Units
                    .ToArray2D()
                    .Map(ParseUnitChar)
                    .Map(CreateUnitDescritpion)
                ;
            
            var tiles = 
                    Tiles
                    .ToArray2D()
                    .Map(ParseLandscapeChar)
                    .Map(CreateTile)
                ;

            return new DBoard(tiles, entrance);
    
            MLandscapeType ParseLandscapeChar(char c) => the_landscape_types_map[c];

            Possible<MUnitType> ParseUnitChar(char c) => c == '-' ? Possible.Empty<MUnitType>() : the_unit_types_map[c];
                
            DTile CreateTile(MLandscapeType t, Index2D i) => 
                new DTile(t, SelectContent(i));

            //TODO: generate random loot from settings?
            Possible<DUnit> CreateUnitDescritpion(Possible<MUnitType> arg) => 
                arg.Select(type => new DUnit(type, the_native_faction, type.s_Initial_Inventory_Content, new List<Possible<DUnit>>()))
            ; 
            
            DTileOccupant SelectContent(Index2D i)
            {
                if (i == entrance.Position)
                    return DStructure.Create.Entrance(entrance.Orientation);
                if (i == exit.Position)
                    return DStructure.Create.Exit(exit.Orientation);

                return units.Get(i).has_a_Value(out var unit) 
                    ? (DTileOccupant) unit 
                    : TheVoid.Instance;
            }
            
            Spacial2D ToSpacial2D(Spacial2DData data) => new Spacial2D(data.Position.ToIndex2D(), data.Orientation);
        }
    }
}
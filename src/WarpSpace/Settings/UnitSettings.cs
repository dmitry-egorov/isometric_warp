using System;
using System.Linq;
using Lanski.UnityExtensions;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;

namespace WarpSpace.Settings
{
    [Serializable]
    public class UnitSettings
    {
        public UnitTypeSettings Type;
        public FactionSettings Faction;
        public int InventoryContent;
        public OptionalUnitSettings[] Bay_Units;
        
        public DUnit s_Description_With(UnitTypeSettingsHolder types_map, FactionSettingsHolder faction_map) => 
            new DUnit
            (
                types_map.s_Model_Of(Type), 
                InventoryHelper.Possible_Stuff_From(InventoryContent), 
                Bay_Units.Select(ous => ous.s_Possible.Select(us => us.s_Description_With(types_map, faction_map))).ToArray(),
                faction_map.s_Model_Of(Faction)
            );
    }
    
    [Serializable]
    public class OptionalUnitSettings : Optional<UnitSettings>{}
}
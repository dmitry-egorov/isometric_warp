using System;
using System.Collections.Generic;
using System.Linq;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;

namespace WarpSpace.Settings
{
    [Serializable]
    public class UnitSettings
    {
        public UnitTypeSettings Type;
        public int InventoryContent;
        public UnitTypeSettings[] Bay_Units;
        
        public DUnit s_Description_With(MFaction faction) => 
            new DUnit
            (
                UnitTypeSettings.s_Model_Of(Type),
                faction, 
                InventoryHelper.Possible_Stuff_From(InventoryContent), 
                Bay_Units.Select(type => new DUnit(UnitTypeSettings.s_Model_Of(type), faction).as_a_Possible()).ToArray()
            )
        ;
    }
}
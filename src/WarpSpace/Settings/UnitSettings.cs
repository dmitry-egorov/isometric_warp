using System;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;

namespace WarpSpace.Settings
{
    [Serializable]
    public class UnitSettings
    {
        public UnitTypeSettings Type;
        public int InventoryContent;
        
        public DUnit s_Description_With(MFaction faction) => 
            new DUnit
            (
                UnitTypeSettings.s_Model_Of(Type),
                faction, 
                InventoryHelper.Possible_Stuff_From(InventoryContent) 
            )
        ;
    }
}
using System.Collections.Generic;
using Lanski.Structures;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Descriptions
{
    public struct DUnit
    {
        public readonly MUnitType s_Type;
        public readonly MFaction s_Faction;
        public readonly DStuff s_Inventory_Content;

        public DUnit(MUnitType the_type, MFaction the_faction) : this(the_type, the_faction, DStuff.Empty()) {}

        public DUnit(MUnitType the_type, MFaction the_faction, DStuff the_inventory_content)
        {
            s_Type = the_type;
            s_Inventory_Content = the_inventory_content;
            s_Faction = the_faction;
        }
    }
}
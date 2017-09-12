using System.Collections.Generic;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public struct DUnit
    {
        public readonly UnitType Type;
        public readonly Faction Faction;
        public readonly Possible<DStuff> Inventory_Content;
        public readonly Possible<IReadOnlyList<Possible<DUnit>>> Bay_Content;

        public DUnit(UnitType type, Faction faction, Possible<DStuff> inventory_content, Possible<IReadOnlyList<Possible<DUnit>>> bay_content)
        {
            Type = type;
            Inventory_Content = inventory_content;
            Bay_Content = bay_content;
            Faction = faction;
        }
    }
}
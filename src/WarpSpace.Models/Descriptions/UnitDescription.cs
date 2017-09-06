using System.Collections.Generic;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public struct UnitDescription
    {
        public readonly UnitType Type;
        public readonly Faction Faction;
        public readonly Possible<Stuff> Inventory_Content;
        public readonly Possible<IReadOnlyList<Possible<UnitDescription>>> Bay_Content;

        public UnitDescription(UnitType type, Faction faction, Possible<Stuff> inventory_content, Possible<IReadOnlyList<Possible<UnitDescription>>> bay_content)
        {
            Type = type;
            Inventory_Content = inventory_content;
            Bay_Content = bay_content;
            Faction = faction;
        }
    }
}
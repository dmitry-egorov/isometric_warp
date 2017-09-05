using Lanski.Structures;

namespace WarpSpace.Descriptions
{
    public struct UnitDescription
    {
        public readonly UnitType Type;
        public readonly Faction Faction;
        public readonly InventoryContent? Inventory_Content;
        public readonly Slot<Slot<UnitDescription>[]> Bay_Content;

        public UnitDescription(UnitType type, Faction faction, InventoryContent? inventory_content, Slot<Slot<UnitDescription>[]> bay_content)
        {
            Type = type;
            Inventory_Content = inventory_content;
            Bay_Content = bay_content;
            Faction = faction;
        }
    }
}
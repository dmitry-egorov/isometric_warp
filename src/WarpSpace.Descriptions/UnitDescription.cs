namespace WarpSpace.Descriptions
{
    public struct UnitDescription
    {
        public readonly UnitType Type;
        public readonly Faction Faction;
        public readonly InventoryContent? Inventory_Content;

        public UnitDescription(UnitType type, Faction faction, InventoryContent? inventory_content)
        {
            Type = type;
            Inventory_Content = inventory_content;
            Faction = faction;
        }
    }
}
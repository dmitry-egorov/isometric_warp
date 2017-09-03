namespace WarpSpace.Descriptions
{
    public struct UnitDescription
    {
        public readonly UnitType Type;
        public readonly InventoryContent? Inventory_Content;

        public UnitDescription(UnitType type, InventoryContent? inventory_content)
        {
            Type = type;
            Inventory_Content = inventory_content;
        }
    }
}
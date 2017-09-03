using Lanski.Structures;

namespace WarpSpace.Descriptions
{
    public struct StructureDescription
    {
        public readonly Type TheType;
        public readonly Direction2D Orientation;//Debris might not need orientation

        public static StructureDescription Entrance(Direction2D orientation) => new StructureDescription(Type.Entrance, orientation);
        public static StructureDescription Exit(Direction2D orientation) => new StructureDescription(Type.Exit, orientation);
        public static StructureDescription Debris(Direction2D orientation, InventoryContent? loot) => new StructureDescription(Type.Debris, orientation) {_debris_slot = new DebrieStructure(loot)};
        
        private StructureDescription(Type type, Direction2D orientation): this()
        {
            TheType = type;
            Orientation = orientation;
        }

        public bool Is_An_Entrance() => TheType == Type.Entrance;
        public bool Is_An_Exit() => TheType == Type.Exit;
        public bool Is_A_Debris(out DebrieStructure debris) => _debris_slot.Has_a_Value(out debris) && TheType == Type.Debris;//Note: Assert the type?

        public struct DebrieStructure
        {
            public readonly InventoryContent? Loot;
            public DebrieStructure(InventoryContent? possible_loot) => Loot = possible_loot;
        }

        private DebrieStructure? _debris_slot;
        
        public enum Type
        {
            Entrance,
            Exit,
            Debris
        }
    }
}
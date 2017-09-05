using System.Diagnostics.Contracts;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public struct StructureDescription
    {
        public readonly Direction2D Orientation;//Debris might not need orientation

        public static class Create
        {
            public static StructureDescription Entrance(Direction2D orientation) => new StructureDescription(orientation) { _variant = new Entrance() };
            public static StructureDescription Exit(Direction2D orientation) => new StructureDescription(orientation) { _variant = new Exit() };
            public static StructureDescription Debris(Direction2D orientation, Slot<InventoryContent> loot) => new StructureDescription(orientation) { _variant = new Debris(loot) };            
        }
        
        private StructureDescription(Direction2D orientation): this() => Orientation = orientation;

        [Pure] public bool Is_An_Entrance() => _variant.Is_a_T1();
        [Pure] public bool Is_An_Exit() => _variant.Is_a_T2();
        [Pure] public bool Is_A_Debris() => _variant.Is_a_T3();
        [Pure] public bool Is_A_Debris(out Debris debris) => _variant.Is_a_T3(out debris);//Note: Assert the type?
        
        public struct Exit {}
        public struct Entrance {}

        public struct Debris
        {
            public readonly Slot<InventoryContent> Loot;
            public Debris(Slot<InventoryContent> possible_loot) => Loot = possible_loot;
        }

        private Or<Entrance, Exit, Debris> _variant;
    }
}
using System.Diagnostics.Contracts;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public struct StructureDescription
    {
        public readonly Direction2D s_Orientation;//Debris might not need orientation

        public static class Create
        {
            public static StructureDescription Entrance(Direction2D orientation) => new StructureDescription(orientation) { the_variant = new Entrance() };
            public static StructureDescription Exit(Direction2D orientation) => new StructureDescription(orientation) { the_variant = new Exit() };
            public static StructureDescription Debris(Direction2D orientation, Possible<Stuff> loot) => new StructureDescription(orientation) { the_variant = new Debris(loot) };            
        }
        
        private StructureDescription(Direction2D orientation): this() => s_Orientation = orientation;

        [Pure] public bool is_an_Entrance() => the_variant.is_a_T1();
        [Pure] public bool is_an_Exit() => the_variant.is_a_T2();
        [Pure] public bool is_a_Debris() => the_variant.is_a_T3();
        [Pure] public bool is_a_Debris(out Debris debris) => the_variant.is_a_T3(out debris);//Note: Assert the type?
        [Pure] public Debris must_be_a_Debris() => the_variant.must_be_a_T3();//Note: Assert the type?
        
        
        public struct Exit {}
        public struct Entrance {}

        public struct Debris
        {
            public Possible<Stuff> s_Loot() => the_loot;

            public Debris(Possible<Stuff> possible_loot) => the_loot = possible_loot;

            private readonly Possible<Stuff> the_loot;
        }

        private Or<Entrance, Exit, Debris> the_variant;
    }
}
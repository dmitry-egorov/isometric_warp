using System.Diagnostics.Contracts;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public struct DStructure
    {
        public readonly Direction2D s_Orientation;//Debris might not need orientation

        public static class Create
        {
            public static DStructure Entrance(Direction2D orientation) => new DStructure(orientation) { the_variant = new Entrance() };
            public static DStructure Exit(Direction2D orientation) => new DStructure(orientation) { the_variant = new Exit() };
            public static DStructure Debris(Direction2D orientation, Possible<DStuff> loot) => new DStructure(orientation) { the_variant = new Debris(loot) };            
        }
        
        private DStructure(Direction2D orientation): this() => s_Orientation = orientation;

        [Pure] public bool is_an_Entrance() => the_variant.is_a_T1();
        [Pure] public bool is_an_Exit() => the_variant.is_a_T2();
        [Pure] public bool is_a_Debris() => the_variant.is_a_T3();
        [Pure] public bool is_a_Debris(out Debris debris) => the_variant.is_a_T3(out debris);//Note: Assert the type?
        [Pure] public Debris must_be_a_Debris() => the_variant.must_be_a_T3();//Note: Assert the type?

        public override string ToString() => the_variant.ToString();

        private Or<Entrance, Exit, Debris> the_variant;

        public struct Exit { public override string ToString() => "Exit"; }
        public struct Entrance { public override string ToString() => "Entrance"; }

        public struct Debris
        {
            public Possible<DStuff> s_Loot => the_loot;

            public Debris(Possible<DStuff> possible_loot) => the_loot = possible_loot;
            
            public override string ToString() => "Debris"; 

            private readonly Possible<DStuff> the_loot;
        }
    }
}
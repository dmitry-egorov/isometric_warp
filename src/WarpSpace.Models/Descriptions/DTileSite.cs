using System.Diagnostics.Contracts;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board;

namespace WarpSpace.Models.Descriptions
{
    public struct DTileSite
    {
        public DTileSite(Or<DUnit, DStructure, TheVoid> variant)
        {
            the_variant = variant;
        }

        [Pure] public bool Is_a_Unit(out DUnit unit) => the_variant.is_a_T1(out unit);
        [Pure] public bool Is_a_Structure(out DStructure structure) => the_variant.is_a_T2(out structure);
        [Pure] public bool Is_Empty() => the_variant.is_a_T3();

        [Pure] public DUnit Must_Be_a_Unit() => the_variant.must_be_a_T1();
        
        public static implicit operator DTileSite(DUnit unit) => new DTileSite(unit);
        public static implicit operator DTileSite(DStructure structure) => new DTileSite(structure);
        public static implicit operator DTileSite(TheVoid the_void) => new DTileSite(the_void);

        private readonly Or<DUnit, DStructure, TheVoid> the_variant;
    }
}
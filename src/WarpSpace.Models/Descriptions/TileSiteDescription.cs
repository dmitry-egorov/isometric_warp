using System.Diagnostics.Contracts;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board;

namespace WarpSpace.Models.Descriptions
{
    public struct TileSiteDescription
    {
        public TileSiteDescription(Or<UnitDescription, StructureDescription, TheVoid> variant)
        {
            the_variant = variant;
        }

        [Pure] public bool Is_a_Unit(out UnitDescription unit) => the_variant.is_a_T1(out unit);
        [Pure] public bool Is_a_Structure(out StructureDescription structure) => the_variant.is_a_T2(out structure);
        [Pure] public bool Is_Empty() => the_variant.is_a_T3();

        [Pure] public UnitDescription Must_Be_a_Unit() => the_variant.must_be_a_T1();
        
        public static implicit operator TileSiteDescription(UnitDescription unit) => new TileSiteDescription(unit);
        public static implicit operator TileSiteDescription(StructureDescription structure) => new TileSiteDescription(structure);
        public static implicit operator TileSiteDescription(TheVoid the_void) => new TileSiteDescription(the_void);

        private readonly Or<UnitDescription, StructureDescription, TheVoid> the_variant;
    }
}
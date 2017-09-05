using System.Diagnostics.Contracts;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board;

namespace WarpSpace.Models.Descriptions
{
    public struct TileContentDescription
    {
        public readonly Or<UnitDescription, StructureDescription, TheVoid> Variant;

        public static class Create
        {
            public static TileContentDescription From(UnitDescription unit) => new TileContentDescription(unit);
            public static TileContentDescription From(StructureDescription structure) => new TileContentDescription(structure);
            public static TileContentDescription Empty() => new TileContentDescription(TheVoid.Instance);
        }

        public TileContentDescription(Or<UnitDescription, StructureDescription, TheVoid> variant)
        {
            Variant = variant;
        }

        [Pure] public bool Is_a_Unit(out UnitDescription unit) => Variant.Is_a_T1(out unit);
        [Pure] public bool Is_a_Structure(out StructureDescription structure) => Variant.Is_a_T2(out structure);
        [Pure] public bool Is_Empty() => Variant.Is_a_T3();

        [Pure] public UnitDescription Must_Be_a_Unit() => Variant.Must_Be_a_T1();
        
        public static implicit operator TileContentDescription(UnitDescription unit) => new TileContentDescription(unit);
        public static implicit operator TileContentDescription(StructureDescription structure) => new TileContentDescription(structure);
        public static implicit operator TileContentDescription(TheVoid the_void) => new TileContentDescription(the_void);

    }
}
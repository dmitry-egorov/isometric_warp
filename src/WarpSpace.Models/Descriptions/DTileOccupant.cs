using System;
using System.Diagnostics.Contracts;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public struct DTileOccupant : IEquatable<DTileOccupant>
    {
        public static implicit operator DTileOccupant(DUnit the_unit) => new DTileOccupant(the_unit);
        public static implicit operator DTileOccupant(DStructure structure) => new DTileOccupant(structure);
        public static implicit operator DTileOccupant(TheVoid the_void) => new DTileOccupant(the_void);

        private DTileOccupant(Or<DUnit, DStructure, TheVoid> variant)
        {
            the_variant = variant;
        }

        [Pure] public bool is_a_Unit(out DUnit the_unit) => the_variant.is_a_T1(out the_unit);
        [Pure] public bool is_a_Structure(out DStructure structure) => the_variant.is_a_T2(out structure);
        [Pure] public bool is_Empty() => the_variant.is_a_T3();

        [Pure] public DUnit must_be_a_Unit() => the_variant.must_be_a_T1();
        
        private readonly Or<DUnit, DStructure, TheVoid> the_variant;

        public bool Equals(DTileOccupant other) => the_variant.Equals(other.the_variant);
    }
}
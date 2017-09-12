using System;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static Lanski.Structures.Semantics;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct TileSite : IEquatable<TileSite>
    {
        public TileSite(MUnitLocation unit_slot) { its_variant = unit_slot; }
        public TileSite(MStructure structure) { its_variant = structure; }

        public bool is_a_Location(out MUnitLocation unit_slot) => its_variant.is_a_T1(out unit_slot);
        public bool is_a_Structure() => its_variant.is_a_T2();
        public bool is_a_Structure(out MStructure structure) => its_variant.is_a_T2(out structure);

        public bool has_a_Unit(out MUnit unit) => 
            semantic_resets(out unit)
            && this.is_a_Location(out var the_location)
            && the_location.has_a_Unit(out unit)
        ;

        public static implicit operator TileSite(MUnitLocation location) => new TileSite(location);
        public static implicit operator TileSite(MStructure structure) => new TileSite(structure);

        public bool Equals(TileSite other) => its_variant.Equals(other.its_variant);
        
        private readonly Or<MUnitLocation, MStructure> its_variant;
    }
}
using System;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static Lanski.Structures.Semantics;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct TileSite : IEquatable<TileSite>
    {
        public readonly Or<MLocation, MStructure> Variant;
        
        public TileSite(MLocation unit_slot) { Variant = unit_slot; }
        public TileSite(MStructure structure) { Variant = structure; }

        public bool is_Empty => !is_Occupied;
        public bool is_Occupied => is_a_Structure() || is_a_Location(out var unit_slot) && unit_slot.is_Occupied();

        public bool is_a_Location() => Variant.is_a_T1();
        public bool is_a_Structure() => Variant.is_a_T2();
        public bool is_a_Location(out MLocation unit_slot) => Variant.is_a_T1(out unit_slot);
        public bool is_a_Structure(out MStructure structure) => Variant.is_a_T2(out structure);
        public Possible<MLocation> as_a_Location() => Variant.as_a_T1();
        public Possible<MStructure> as_a_Structure() => Variant.as_a_T2();
        public MLocation must_be_a_Location() => Variant.must_be_a_T1();

        public Possible<MUnit> s_possible_Unit() => as_a_Location().SelectMany(the_location => the_location.s_Unit);
        public bool has_a_Unit() => is_a_Location(out var unit_slot) && unit_slot.has_a_Unit(); 
        public bool has_a_Unit(out MUnit unit) => 
            semantic_resets(out unit) 
            && is_a_Location(out var unit_slot) 
            && unit_slot.has_a_Unit(out unit)
        ;

        public bool @is(MLocation location) => is_a_Location(out var unit_slot) && unit_slot == location;

        public static implicit operator TileSite(MLocation location) => new TileSite(location);
        public static implicit operator TileSite(MStructure structure) => new TileSite(structure);

        public bool Equals(TileSite other) => Variant.Equals(other.Variant);

        
    }
}
using System;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static Lanski.Structures.Semantics;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct TileSite
    {
        public readonly Or<MLocation, MStructure> Variant;
        
        public TileSite(MLocation unit_slot) { Variant = unit_slot; }
        public TileSite(MStructure structure) { Variant = structure; }

        public bool Is_Empty() => !Is_Occupied();
        public bool Is_Occupied() => Is_a_Structure() || Is_a_Location(out var unit_slot) && unit_slot.Is_Occupied();

        public bool Is_a_Location() => Variant.Is_a_T1();
        public bool Is_a_Structure() => Variant.Is_a_T2();
        public bool Is_a_Location(out MLocation unit_slot) => Variant.Is_a_T1(out unit_slot);
        public bool Is_a_Structure(out MStructure structure) => Variant.Is_a_T2(out structure);
        public Possible<MLocation> As_a_Location() => Variant.As_a_T1();
        public Possible<MStructure> As_a_Structure() => Variant.As_a_T2();
        public MLocation Must_Be_a_Location() => Variant.Must_Be_a_T1();

        public bool Has_a_Unit() => Is_a_Location(out var unit_slot) && unit_slot.Has_a_Unit(); 
        public bool Has_a_Unit(out MUnit unit) => 
            Set_Default_Value_To(out unit) 
            && Is_a_Location(out var unit_slot) 
            && unit_slot.Has_a_Unit(out unit)
        ;

        public bool Is(MLocation location) => Is_a_Location(out var unit_slot) && unit_slot == location;

        public static implicit operator TileSite(MLocation location) => new TileSite(location);
        public static implicit operator TileSite(MStructure structure) => new TileSite(structure);
    }
}
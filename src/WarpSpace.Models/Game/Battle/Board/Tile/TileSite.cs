using System;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static Lanski.Structures.Semantics;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct TileSite
    {
        public readonly Or<LocationModel, StructureModel> Variant;
        
        public TileSite(LocationModel unit_slot) { Variant = unit_slot; }
        public TileSite(StructureModel structure) { Variant = structure; }

        public bool Is_Empty() => !Is_Occupied();
        public bool Is_Occupied() => Is_a_Structure() || Is_a_Location(out var unit_slot) && unit_slot.Is_Occupied();

        public bool Is_a_Location() => Variant.Is_a_T1();
        public bool Is_a_Structure() => Variant.Is_a_T2();
        public bool Is_a_Location(out LocationModel unit_slot) => Variant.Is_a_T1(out unit_slot);
        public bool Is_a_Structure(out StructureModel structure) => Variant.Is_a_T2(out structure);
        public Slot<LocationModel> As_a_Location() => Variant.As_a_T1();
        public Slot<StructureModel> As_a_Structure() => Variant.As_a_T2();
        public LocationModel Must_Be_a_Location() => Variant.Must_Be_a_T1();

        public bool Has_a_Unit() => Is_a_Location(out var unit_slot) && unit_slot.Has_a_Unit(); 
        public bool Has_a_Unit(out UnitModel unit) => 
            Set_Default_Value_To(out unit) 
            && Is_a_Location(out var unit_slot) 
            && unit_slot.Has_a_Unit(out unit)
        ;

        public bool Is(LocationModel location) => Is_a_Location(out var unit_slot) && unit_slot == location;

        public static implicit operator TileSite(LocationModel location) => new TileSite(location);
        public static implicit operator TileSite(StructureModel structure) => new TileSite(structure);
    }
}
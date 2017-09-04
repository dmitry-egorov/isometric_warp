using System;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static Lanski.Structures.Semantics;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct TileSite
    {
        public TileSite(LocationModel unit_slot) { _variant = unit_slot; }
        public TileSite(StructureModel structure) { _variant = structure; }

        private Or<LocationModel, StructureModel> _variant;

        public bool Is_Empty() => !Is_Occupied();
        public bool Is_Occupied() => Is_a_Structure() || Is_a_Unit_Slot(out var unit_slot) && unit_slot.Is_Occupied();

        public bool Is_a_Unit_Slot() => _variant.Is_a_T1();
        public bool Is_a_Structure() => _variant.Is_a_T2();
        public bool Is_a_Unit_Slot(out LocationModel unit_slot) => _variant.Is_a_T1(out unit_slot);
        public bool Is_a_Structure(out StructureModel structure) => _variant.Is_a_T2(out structure);
        public Slot<StructureModel> As_a_Structure() => Is_a_Structure(out var structure) ? structure.As_a_Slot() : Slot.Empty<StructureModel>();
        public Slot<LocationModel> As_a_Unit_Slot() => Is_a_Unit_Slot(out var unit_slot) ? unit_slot.As_a_Slot() : Slot.Empty<LocationModel>();
        public LocationModel Must_Be_a_Unit_Slot() => As_a_Unit_Slot().Must_Have_a_Value();

        public bool Has_a_Unit() => Is_a_Unit_Slot(out var unit_slot) && unit_slot.Has_a_Unit(); 
        public bool Has_a_Unit(out UnitModel unit) => 
            Set_Default_Value_To(out unit) 
            && Is_a_Unit_Slot(out var unit_slot) 
            && unit_slot.Has_a_Unit(out unit);

        public bool Is(LocationModel location) => Is_a_Unit_Slot(out var unit_slot) && unit_slot == location;

    }
}
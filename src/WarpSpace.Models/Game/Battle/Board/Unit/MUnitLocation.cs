using System;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public struct MUnitLocation: IEquatable<MUnitLocation>
    {
        public static implicit operator MUnitLocation(MTile the_tile) => new MUnitLocation(the_tile);
        public static implicit operator MUnitLocation(MBaySlot the_bay_slot) => new MUnitLocation(the_bay_slot);
        
        public MUnitLocation(Or<MTile, MBaySlot> the_variant) => its_variant = the_variant;

        public bool is_Adjacent_To(MUnitLocation the_other) => 
            this.is_a_Tile(out var the_tile) 
            && the_other.is_a_Tile(out var the_others_tile) 
            && the_tile.is_Adjacent_To(the_others_tile)
        ;

        public MTile s_Tile => its_tile;

        public Passability s_Passability_With(MChassisType the_chassis_type) => this.is_a_Bay_Slot() ? Passability.All_Moves : this.must_be_a_Tile().s_Passability_With(the_chassis_type); 
        public bool is_Passable_By(MChassisType the_chassis_type) => this.is_a_Bay_Slot() || this.is_a_Tile(out var the_tile) && the_tile.is_Passable_By(the_chassis_type); 
        public bool is_Accessible_From(MUnitLocation the_other_location) => its_tile.is_Adjacent_To(the_other_location.s_Tile);
        public bool is_Adjacent_To(MStructure structure) => is_a_Tile(out var own_tile) && own_tile.is_Adjacent_To(structure.s_Location);
        
        public Possible<Index2D> s_Possible_Position => its_variant.as_a_T1().Select(x => x.s_Position);
        public bool is_a_Tile() => its_variant.is_a_T1();
        public bool is_a_Tile(out MTile tile) => its_variant.is_a_T1(out tile);
        public bool is_a_Bay_Slot() => its_variant.is_a_T2();
        public bool is_a_Bay_Slot(out MBaySlot the_bay_slot) => its_variant.is_a_T2(out the_bay_slot);
        public bool is_a_Bay(out MBay the_bay) => Flow.default_as(out the_bay) && its_variant.is_a_T2(out var the_bay_slot) && the_bay_slot.s_Bay.@as(out the_bay);
        public MTile must_be_a_Tile() => its_variant.must_be_a_T1();
        public MBaySlot must_be_a_Bay_Slot() => its_variant.must_be_a_T2();
        public MBay must_be_a_Bay() => this.must_be_a_Bay_Slot().s_Bay;

        public bool Equals(MUnitLocation other) => its_variant.Equals(other.its_variant);

        public bool is_Empty() => 
            is_a_Tile(out var the_tile) && the_tile.is_Empty() ||
            is_a_Bay_Slot(out var the_bay_slot) && the_bay_slot.is_Empty()
        ;

        public void s_Occupant_Becomes(Possible<MUnit> the_new_unit)
        {
            if (this.is_a_Tile(out var the_tile))
            {
                the_tile.s_Occupant_Becomes(the_new_unit.has_a_Value(out var the_unit) ? the_unit : MTileOccupant.Empty);
            }
            else if (this.is_a_Bay_Slot(out var the_bay_slot))
            {
                the_bay_slot.s_Occupant_Becomes(the_new_unit);
            }
            else
            {
                throw new InvalidOperationException("Unknown variant");
            }
        }

        public void s_Occupant_Becomes_Empty() => this.s_Occupant_Becomes(Possible.Empty<MUnit>());

        private MTile its_tile => this.is_a_Tile(out var self) ? self : this.must_be_a_Bay().s_Owner.must_be_At_a_Tile();
        
        private readonly Or<MTile, MBaySlot> its_variant;
    }
}
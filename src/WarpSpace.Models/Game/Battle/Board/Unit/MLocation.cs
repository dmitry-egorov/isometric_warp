using System;
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MLocation: IEquatable<MLocation>
    {
        public MLocation(Or<MTile, MBay> the_owner, SignalGuard the_signal_guard): this(the_owner, Possible.Empty<MUnit>(), the_signal_guard) {}
        public MLocation(Or<MTile, MBay> the_owner, Possible<MUnit> the_possible_unit, SignalGuard the_signal_guard)
        {
            its_owner = the_owner;
            
            its_possible_units_cell = new GuardedCell<Possible<MUnit>>(the_possible_unit, the_signal_guard);
            its_has_a_unit_cell = its_possible_units_cell.Select(pu => pu.has_a_Value());
        }

        public bool is_Adjacent_To(MLocation the_other) => 
            this.is_a_Tile(out var the_tile) 
            && the_other.is_a_Tile(out var the_others_tile) 
            && the_tile.is_Adjacent_To(the_others_tile)
        ;

        public Possible<MUnit> s_Possible_Unit => its_possible_unit;
        public MTile s_Tile => its_tile;
        public ICell<bool> s_has_a_unit_cell => its_has_a_unit_cell;

        public bool is_Passable_By(ChassisType the_chassis_type) => this.is_a_Bay() || this.is_a_Tile(out var the_tile) && the_tile.is_Passable_By(the_chassis_type); 
        public bool is_Accessible_From(MLocation the_other_location) => its_tile.is_Adjacent_To(the_other_location.s_Tile);
        public bool is_Adjacent_To(MStructure structure) => is_a_Tile(out var own_tile) && own_tile.is_Adjacent_To(structure.s_Location);
        public void must_be_Empty() => this.is_Empty().Must_Be_True();
        public bool is_Occupied() => !this.is_Empty();
        public bool has_a_Unit() => is_Occupied();
        public bool has_a_Unit(out MUnit unit) => its_possible_unit.has_a_Value(out unit);
        public bool is_Empty() => its_possible_unit.Has_Nothing();

        public Possible<MTile> as_a_Tile() => its_owner.as_a_T1();
        public bool @is(MTile tile) => this.is_a_Tile(out var own_tile) && own_tile == tile;
        public bool is_a_Tile() => its_owner.is_a_T1();
        public bool is_a_Tile(out MTile tile) => its_owner.is_a_T1(out tile);
        public bool is_a_Bay() => its_owner.is_a_T2();
        public bool is_a_Bay(out MBay bay) => its_owner.is_a_T2(out bay);
        public MTile must_be_a_Tile() => its_owner.must_be_a_T1();
        public MBay must_be_a_Bay() => its_owner.must_be_a_T2();
        
        public bool Equals(MLocation other) => ReferenceEquals(this, other);

        internal void s_Occupant_Becomes(MUnit unit)
        {
            this.is_Empty().Otherwise_Throw("Can't set a unit on an occupied location.");
            
            its_occupant_becomes(unit);
        }

        internal void Becomes_Empty()
        {
            this.is_Occupied().Otherwise_Throw("Can't reset a unit on an empty location.");

            its_occupant_becomes(Possible.Empty<MUnit>());
        }

        private void its_occupant_becomes(Possible<MUnit> possible_occupant) => its_possible_unit = possible_occupant;

        private MTile its_tile => this.is_a_Tile(out var self) ? self : this.must_be_a_Bay().s_Owner.must_be_At_a_Tile();

        private Possible<MUnit> its_possible_unit
        {
            get => its_possible_units_cell.s_Value;
            set => its_possible_units_cell.s_Value = value;
        }

        private readonly Or<MTile, MBay> its_owner;
        private readonly GuardedCell<Possible<MUnit>> its_possible_units_cell;
        private readonly ICell<bool> its_has_a_unit_cell;
    }
}
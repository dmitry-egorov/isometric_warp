using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MBaySlot
    {
        public MBaySlot(int the_index, MBay the_bay, SignalGuard the_signal_guard)
        {
            its_index = the_index;
            its_bay = the_bay;
            its_possible_units_cell = new GuardedCell<Possible<MUnit>>(Possible.Empty<MUnit>(), the_signal_guard);
        }

        public MBay s_Bay => its_bay;
        
        public Possible<MUnit> s_Possible_Unit => its_possible_units_cell.s_Value;
        public ICell<Possible<MUnit>> s_Possible_Units_Cell => its_possible_units_cell;

        public bool is_Empty() => !it_has_a_unit();

        public override string ToString() => $"Bay slot {its_index} of {its_bay.s_Owner}";

        internal void s_Occupant_Becomes(Possible<MUnit> unit)
        {
            (unit.has_a_Value() == this.is_Empty()).Otherwise_Throw("Can't set a unit.");
            
            its_occupant_becomes(unit);
        }

        private Possible<MUnit> its_possible_unit
        {
            get => its_possible_units_cell.s_Value;
            set => its_possible_units_cell.s_Value = value;
        }

        private bool it_has_a_unit() => its_possible_unit.has_a_Value();

        private void its_occupant_becomes(Possible<MUnit> possible_occupant) => its_possible_unit = possible_occupant;

        private readonly int its_index;
        private readonly MBay its_bay;
        private readonly GuardedCell<Possible<MUnit>> its_possible_units_cell;

        public bool is_Accessible_From(MTile the_tile)
        {
            return s_Bay.s_Owner.is_Adjacent_To(the_tile);
        }
    }
}
using Lanski.Reactive;
using Lanski.Structures;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MBaySlot
    {
        public MBaySlot(MBay the_bay, SignalGuard the_signal_guard)
        {
            its_bay = the_bay;
            its_possible_units_cell = new GuardedCell<Possible<MUnit>>(Possible.Empty<MUnit>(), the_signal_guard);
        }

        public MBay s_Bay => its_bay;
        
        public Possible<MUnit> s_Possible_Unit => its_possible_units_cell.s_Value;
        public ICell<Possible<MUnit>> s_Possible_Units_Cell => its_possible_units_cell;

        public bool is_Empty() => !it_has_a_unit();

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

        private readonly MBay its_bay;
        private readonly GuardedCell<Possible<MUnit>> its_possible_units_cell;
    }
}
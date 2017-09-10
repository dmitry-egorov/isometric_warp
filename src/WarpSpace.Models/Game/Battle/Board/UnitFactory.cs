using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class UnitFactory
    {
        public IStream<MUnit> s_Unit_Creations_Stream => its_unit_creations_stream;

        public UnitFactory(SignalGuard the_signal_guard)
        {
            this.the_signal_guard = the_signal_guard;
            its_unit_creations_stream = new GuardedStream<MUnit>(the_signal_guard);
        }

        public bool can_Create_a_Unit_At(MLocation location) => location.is_Empty();
        
        public void Creates_a_Unit(UnitDescription desc, MLocation initial_location)
        {
            this.can_Create_a_Unit_At(initial_location).Otherwise_Throw("Can't create a unit at the location");

            var unit = new MUnit(desc.Type, desc.Faction, desc.Inventory_Content, initial_location, the_signal_guard);

            initial_location.s_Occupant_Becomes(unit);

            it_creates_the_units_in_the_bay(desc, unit);

            it_signals_the_creation(unit);
        }

        private void it_creates_the_units_in_the_bay(UnitDescription desc, MUnit unit)
        {
            if (!unit.has_a_Bay(out var bay)) 
                return;
            
            var content = desc.Bay_Content.must_have_a_Value();
            (bay.Size >= content.Count).Otherwise_Throw("Actual unit's bay size is smaller then the described content size");

            for (var i = 0; i < content.Count; i++)
            {
                var possible_unit_in_the_bay = content[i];
                if (possible_unit_in_the_bay.has_a_Value(out var unit_in_the_bay))
                    Creates_a_Unit(unit_in_the_bay, bay[i].must_have_a_Value());
            }
        }
        
        void it_signals_the_creation(MUnit unit) => its_unit_creations_stream.Next(unit);

        private readonly GuardedStream<MUnit> its_unit_creations_stream;
        private readonly SignalGuard the_signal_guard;
    }
}
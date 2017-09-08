using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class UnitFactory
    {
        public IStream<MUnit> Stream_Of_Unit_Creations => the_stream_of_unit_creations;

        public UnitFactory(SignalGuard the_signal_guard)
        {
            this.its_signal_guard = the_signal_guard;
            the_stream_of_unit_creations = new GuardedStream<MUnit>(the_signal_guard);
        }

        public bool Can_Create_a_Unit_At(MLocation location) => location.is_Empty();
        
        public void Creates_a_Unit(UnitDescription desc, MLocation initial_location)
        {
            Can_Create_a_Unit_At(initial_location).Otherwise_Throw("Can't create a unit at the location");

            var unit = new MUnit(desc.Type, desc.Faction, desc.Inventory_Content, initial_location, its_signal_guard);

            initial_location.s_Occupant_Becomes(unit);
            
            Signal_the_Creation(unit);

            Create_Units_In_The_Bay(desc, unit);
        }

        private void Create_Units_In_The_Bay(UnitDescription desc, MUnit unit)
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
        
        void Signal_the_Creation(MUnit unit)
        {
            the_stream_of_unit_creations.Next(unit);                    
        }

        private readonly GuardedStream<MUnit> the_stream_of_unit_creations;
        private readonly SignalGuard its_signal_guard;
    }
}
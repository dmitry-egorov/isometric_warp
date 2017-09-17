using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class MUnitFactory
    {
        public IStream<MUnit> Created_a_Unit => it_created_a_unit;

        public MUnitFactory(MBoard the_owner, MGame the_game, SignalGuard the_signal_guard)
        {
            this.the_signal_guard = the_signal_guard;
            this.the_game = the_game;
            its_owner = the_owner;
            it_created_a_unit = new GuardedStream<MUnit>(the_signal_guard);
        }

        public void Creates_a_Unit(DUnit desc, MUnitLocation initial_location)
        {
            initial_location.is_Empty().Otherwise_Throw("Can't create a unit, since it's initial location is not emoty");

            var the_new_unit = new MUnit(its_last_id, desc, initial_location, the_game, the_signal_guard);
            its_last_id++;

            initial_location.s_Occupant_Becomes(the_new_unit);

            it_creates_the_units_in_the_bay(desc, the_new_unit);

            its_owner.Adds_a_Unit(the_new_unit);
            
            it_signals_the_creation(the_new_unit);
        }

        private void it_creates_the_units_in_the_bay(DUnit desc, MUnit unit)
        {
            if (!unit.has_a_Bay(out var bay)) 
                return;
            
            var the_bay_units = desc.s_Bay_Units;
            (bay.Size >= the_bay_units.Count).Otherwise_Throw("Actual unit's bay size is smaller then the described content size");

            for (var i = 0; i < the_bay_units.Count; i++)
            {
                var possible_unit_in_the_bay = the_bay_units[i];
                if (possible_unit_in_the_bay.has_a_Value(out var unit_in_the_bay))
                    Creates_a_Unit(unit_in_the_bay, bay[i]);
            }
        }
        
        void it_signals_the_creation(MUnit unit) => it_created_a_unit.Next(unit);

        private readonly GuardedStream<MUnit> it_created_a_unit;
        private readonly MBoard its_owner;
        private readonly MGame the_game;
        private readonly SignalGuard the_signal_guard;
        private int its_last_id;
    }
}
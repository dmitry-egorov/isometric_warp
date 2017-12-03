using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
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

        public MUnit Creates_a_Unit(DUnit desc, MTile initial_location)
        {
            initial_location.is_Empty().Otherwise_Throw("Can't create a unit, since it's initial location is not emoty");

            var the_new_unit = new MUnit(its_last_id, desc, initial_location, the_game, the_signal_guard);
            its_last_id++;

            initial_location.s_Occupant_Becomes(the_new_unit);

            its_owner.Adds_a_Unit(the_new_unit);
            
            it_signals_the_creation(the_new_unit);

            return the_new_unit;
        }
        
        void it_signals_the_creation(MUnit unit) => it_created_a_unit.Next(unit);

        private readonly GuardedStream<MUnit> it_created_a_unit;
        private readonly MBoard its_owner;
        private readonly MGame the_game;
        private readonly SignalGuard the_signal_guard;
        private int its_last_id;
    }
}
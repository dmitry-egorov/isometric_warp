using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MInventory
    {
        public MInventory(Possible<DStuff> initial_stuff, SignalGuard the_signal_guard)
        {
            its_stuffs_cell = new GuardedCell<Possible<DStuff>>(initial_stuff, the_signal_guard);
        }
        
        public ICell<Possible<DStuff>> s_Stuffs_Cell => its_stuffs_cell;
        public Possible<DStuff> s_Stuff => its_stuff;

        internal void Adds(Possible<DStuff> the_new_stuff) => its_stuff_becomes(this.its_stuff.and(the_new_stuff));

        private void its_stuff_becomes(Possible<DStuff> value) => its_stuffs_cell.s_Value = value;

        private Possible<DStuff> its_stuff => its_stuffs_cell.s_Value;
        private readonly GuardedCell<Possible<DStuff>> its_stuffs_cell;
    }
}
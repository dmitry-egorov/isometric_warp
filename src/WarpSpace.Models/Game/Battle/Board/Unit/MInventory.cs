using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MInventory
    {
        public MInventory(Possible<Stuff> initial_stuff, SignalGuard the_signal_guard)
        {
            its_stuffs_cell = new GuardedCell<Possible<Stuff>>(initial_stuff, the_signal_guard);
        }
        
        public ICell<Possible<Stuff>> s_Stuffs_Cell => its_stuffs_cell;
        public Possible<Stuff> s_Stuff => its_stuff;

        internal void Adds(Possible<Stuff> the_new_stuff) => its_stuff_becomes(this.its_stuff.and(the_new_stuff));

        private void its_stuff_becomes(Possible<Stuff> value) => its_stuffs_cell.s_Value = value;

        private Possible<Stuff> its_stuff => its_stuffs_cell.s_Value;
        private readonly GuardedCell<Possible<Stuff>> its_stuffs_cell;
    }
}
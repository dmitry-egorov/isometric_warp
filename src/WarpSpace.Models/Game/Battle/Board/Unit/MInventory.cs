using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MInventory
    {
        public MInventory(Possible<Stuff> initial_stuff, SignalGuard the_signal_guard)
        {
            s_stuffs_cell = new GuardedCell<Possible<Stuff>>(initial_stuff, the_signal_guard);
        }
        
        public ICell<Possible<Stuff>> s_Stuffs_Cell => it.s_stuffs_cell;
        public Possible<Stuff> s_Stuff => it.s_stuff;

        internal void Adds(Possible<Stuff> the_new_stuff) => it.s_stuff_becomes(it.s_stuff.and(the_new_stuff));

        private void s_stuff_becomes(Possible<Stuff> value) => it.s_stuffs_cell.s_Value = value;

        private MInventory it => this;
        private Possible<Stuff> s_stuff => it.s_stuffs_cell.s_Value;
        private readonly GuardedCell<Possible<Stuff>> s_stuffs_cell;
    }
}
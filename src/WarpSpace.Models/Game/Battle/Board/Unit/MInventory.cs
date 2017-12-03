using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MInventory
    {
        public MInventory(DStuff initial_stuff, SignalGuard the_signal_guard)
        {
            its_stuffs_cell = new GuardedCell<DStuff>(initial_stuff, the_signal_guard);
        }
        
        public ICell<DStuff> s_Stuffs_Cell => its_stuffs_cell;
        public DStuff s_Content => its_stuff;

        internal void Adds(DStuff the_new_stuff) => its_stuff_becomes(this.its_stuff.and(the_new_stuff));

        private void its_stuff_becomes(DStuff value) => its_stuffs_cell.s_Value = value;

        private DStuff its_stuff => its_stuffs_cell.s_Value;
        private readonly GuardedCell<DStuff> its_stuffs_cell;
    }
}
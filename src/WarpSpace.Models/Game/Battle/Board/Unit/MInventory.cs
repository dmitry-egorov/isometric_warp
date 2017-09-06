using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MInventory
    {
        public ICell<Possible<Stuff>> s_Cell_of_Content() => the_contents_cell;

        public Possible<Stuff> s_content
        {
            get => the_contents_cell.s_Value;
            private set => the_contents_cell.s_Value = value;
        }

        public void Adds(Possible<Stuff> new_content)
        {
            s_content = s_content.And(new_content);
        }

        public MInventory(Possible<Stuff> initial_stuff, EventsGuard the_events_guard)
        {
            the_contents_cell = new GuardedCell<Possible<Stuff>>(initial_stuff, the_events_guard);
        }
        
        private readonly GuardedCell<Possible<Stuff>> the_contents_cell;
    }
}
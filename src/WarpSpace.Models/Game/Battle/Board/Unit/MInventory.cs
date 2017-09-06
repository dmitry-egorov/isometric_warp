using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MInventory
    {
        private readonly ValueCell<Possible<Stuff>> s_cell_of_content;
        public ICell<Possible<Stuff>> s_Cell_of_Content() => s_cell_of_content;

        public Possible<Stuff> s_content
        {
            get => s_cell_of_content.s_Value;
            private set => s_cell_of_content.s_Value = value;
        }

        public static MInventory From(Possible<Stuff> initial_stuff) => new MInventory(initial_stuff);

        public void Adds(Possible<Stuff> new_content)
        {
            s_content = s_content.And(new_content);
        }

        public MInventory(Possible<Stuff> initial_stuff)
        {
            s_cell_of_content = new ValueCell<Possible<Stuff>>(initial_stuff);
        }
    }
}
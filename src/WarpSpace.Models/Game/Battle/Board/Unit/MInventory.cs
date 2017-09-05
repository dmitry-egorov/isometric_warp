using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MInventory
    {
        private readonly ValueCell<Possible<InventoryContent>> _content_cell;
        public ICell<Possible<InventoryContent>> Content_Cell => _content_cell;
        public Possible<InventoryContent> Content => Content_Cell.Value;

        public static MInventory From(Possible<InventoryContent> initial_content) => new MInventory(initial_content);

        public void Add(Possible<InventoryContent> content)
        {
            _content_cell.Value = _content_cell.Value.And(content);
        }

        private MInventory(Possible<InventoryContent> content_cell)
        {
            _content_cell = new ValueCell<Possible<InventoryContent>>(content_cell);
        }
    }
}
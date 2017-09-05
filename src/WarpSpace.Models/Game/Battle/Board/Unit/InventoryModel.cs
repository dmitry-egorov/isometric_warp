using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class InventoryModel
    {
        private readonly ValueCell<Slot<InventoryContent>> _content_cell;
        public ICell<Slot<InventoryContent>> Content_Cell => _content_cell;
        public Slot<InventoryContent> Content => Content_Cell.Value;

        public static InventoryModel From(Slot<InventoryContent> initial_content) => new InventoryModel(initial_content);

        public void Add(Slot<InventoryContent> content)
        {
            _content_cell.Value = _content_cell.Value.And(content);
        }

        private InventoryModel(Slot<InventoryContent> content_cell)
        {
            _content_cell = new ValueCell<Slot<InventoryContent>>(content_cell);
        }
    }
}
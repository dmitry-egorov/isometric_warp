using Lanski.Reactive;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class InventoryModel
    {
        private readonly NullableCell<InventoryContent> _content_cell;
        public ICell<InventoryContent?> Content_Cell => _content_cell;
        public InventoryContent? Content => Content_Cell.Value;

        public static InventoryModel From(InventoryContent? initial_content) => new InventoryModel(initial_content);

        public void Add(InventoryContent? content)
        {
            _content_cell.Value += content;
        }

        private InventoryModel(InventoryContent? content_cell)
        {
            _content_cell = new NullableCell<InventoryContent>(content_cell);
        }
    }
}
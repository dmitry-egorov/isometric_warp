using Lanski.Reactive;

namespace WarpSpace.Models.Game.Battle.Player
{
    public class Model
    {
        private readonly RefCell<Board.Unit.Model> _selectedUnitCell = new RefCell<Board.Unit.Model>(null);
        public ICell<Board.Unit.Model> SelectedUnitCell => _selectedUnitCell;//Unit can be null

        public void ExecuteActionAt(Board.Tile.Model tile)
        {
            if (Try_to_select_a_unit_at_the_tile()) 
                return;

            if (Try_to_move_the_selected_unit_to_the_tile())
                return;
            
            //Deselect();

            bool Try_to_select_a_unit_at_the_tile()
            {
                var unit = tile.Unit.Value;
                if (unit == null)
                    return false;
                
                Select(unit);
                return true;
            }

            bool Try_to_move_the_selected_unit_to_the_tile()
            {
                var unit = _selectedUnitCell.Value;
                if (unit == null)
                    return false;
                
                return unit.TryMoveTo(tile);
            }
        }

        private void Select(Board.Unit.Model unit)
        {
            _selectedUnitCell.Value = null;
            _selectedUnitCell.Value = unit;
        }
    }
}
using JetBrains.Annotations;
using UnityEngine;

namespace WarpSpace.World.Player
{
    public class Component: MonoBehaviour
    {
        [CanBeNull] private Unit.Component _selectedUnit;

        public void ExecuteActionAt(Board.Tile.Component tile)
        {
            if (TryToSelectUnitAt(tile)) 
                return;

            if (TryToMoveSelectedUnitTo(tile))
                return;
            
            //Deselect();
        }

        private bool TryToSelectUnitAt(Board.Tile.Component tile)
        {
            var unit = tile.UnitSlot.Unit;
            if (unit == null) 
                return false;

            Deselect();
            
            Select(unit);
            return true;
        }

        private void Select(Unit.Component unit)
        {
            _selectedUnit = unit;
            unit.SetIsSelected(true);
            SetAllAdjacentPassableHighlightsToMove(unit);
        }

        private bool TryToMoveSelectedUnitTo(Board.Tile.Component tile)
        {
            if (_selectedUnit == null) 
                return false;

            var formerTile = _selectedUnit.Tile;

            if (!_selectedUnit.TryMoveTo(tile)) 
                return false;
            
            ResetAllAdjacentHighlightsOf(formerTile);
            SetAllAdjacentPassableHighlightsToMove(_selectedUnit);
            return true;
        }

        private void Deselect()
        {
            if (_selectedUnit == null) 
                return;
            
            _selectedUnit.SetIsSelected(false);
            ResetAllAdjacentHighlightsOf(_selectedUnit.Tile);
            
            _selectedUnit = null;
        }

        private static void SetAllAdjacentPassableHighlightsToMove(Unit.Component unit)
        {
            foreach (var adjacentTile in unit.Tile.AdjacentTiles.Items)
            {
                if (adjacentTile != null && adjacentTile.Landscape.IsPassableBy(unit))
                    adjacentTile.Highlight.SetMove();
            }
        }
        
        private static void ResetAllAdjacentHighlightsOf(Board.Tile.Component tile)
        {
            foreach (var adjacentTile in tile.AdjacentTiles.Items)
            {
                if (adjacentTile != null)
                {
                    adjacentTile.Highlight.Reset();
                }
            }
        }
    }
}
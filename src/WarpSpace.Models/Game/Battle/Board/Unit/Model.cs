using JetBrains.Annotations;
using Lanski.Reactive;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class Model
    {
        public readonly Chassis Chassis;
        
        private readonly RefCell<Tile.Model> _currentTile;
        public ICell<Tile.Model> CurrentTile => _currentTile;//Tile is not null

        public Model(Chassis chassis, Tile.Model initialTile)
        {
            Chassis = chassis;
            _currentTile = new RefCell<Tile.Model>(initialTile);
        }

        public bool CanMoveTo([CanBeNull] Tile.Model destination)
        {
            var source = _currentTile.Value;

            return destination != null 
                && destination.IsPassableBy(Chassis)
                && source.IsAdjacentTo(destination);
        }

        public bool TryMoveTo(Tile.Model tile)
        {
            if (!CanMoveTo(tile))
                return false;
            
            _currentTile.Value = tile;

            return true;
        }
    }
}
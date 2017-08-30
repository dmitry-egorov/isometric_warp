using JetBrains.Annotations;
using Lanski.Reactive;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class ChassisModel
    {
        private readonly ChassisType _chassisType;
        private readonly RefCell<TileModel> _currentTile;
        public ICell<TileModel> CurrentTile => _currentTile; //Tile is not null
        
        public static ChassisModel From(UnitType unitType, TileModel initialTile) => new ChassisModel(unitType.GetChassisType(), initialTile);

        public ChassisModel(ChassisType chassisType, TileModel initialTile)
        {
            _chassisType = chassisType;
            _currentTile = new RefCell<TileModel>(initialTile);
        }
        
        public bool CanMoveTo(TileModel destination)
        {
            var source = _currentTile.Value;

            return destination.IsPassableBy(_chassisType)
                   && source.is_adjacent_to_the(destination);
        }

        public bool TryMoveTo(TileModel tile)
        {
            if (!CanMoveTo(tile))
                return false;
            
            _currentTile.Value = tile;

            return true;
        }
    }
}
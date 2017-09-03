using Lanski.Reactive;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class ChassisModel
    {
        public static ChassisModel From(UnitModel unit, TileModel initial_tile) => new ChassisModel(unit, unit.Type.Get_Chassis_Type(), initial_tile);

        public ICell<TileModel> Current_Tile => _current_tile;

        public bool Can_Move_To(TileModel destination)
        {
            var source = _current_tile.Value;

            return destination.Is_Passable_By(_chassis_type)
                   && source.Is_Adjacent_To(destination);
        }

        public bool Try_to_Move_To(TileModel tile)
        {
            if (!Can_Move_To(tile))
                return false;

            var prev_tile = _current_tile.Value;
            prev_tile.Reset_Unit();
            tile.Set_Unit(_unit);
            
            _current_tile.Value = tile;

            return true;
        }
        
        private ChassisModel(UnitModel unit, ChassisType chassis_type, TileModel initial_tile)
        {
            _chassis_type = chassis_type;
            _unit = unit;
            
            _current_tile = new RefCell<TileModel>(initial_tile);
            
            initial_tile.Set_Unit(unit);
        }

        private readonly UnitModel _unit;
        private readonly ChassisType _chassis_type;
        private readonly RefCell<TileModel> _current_tile;
    }
}
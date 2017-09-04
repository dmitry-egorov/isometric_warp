using Lanski.Reactive;
using WarpSpace.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class HealthModel
    {
        public readonly int TotalHitPoints;
        public ICell<int> Current_Hit_Points_Cell => _current_hit_points_cell;
        public ICell<bool> Is_Alive_Cell { get; }
        internal bool Is_Alive => Is_Alive_Cell.Value;
        internal bool Is_Dead => !Is_Alive;

        public static HealthModel From(UnitType type, UnitModel unit) => new HealthModel(type.GetHitPointsAmount(), unit);

        public void Take(DamageDescription damage)
        {
            _current_hit_points_cell.Value -= damage.Amount;
        }

        private HealthModel(int total_hit_points, UnitModel unit)
        {
            TotalHitPoints = total_hit_points;
            _unit = unit;
            _current_hit_points_cell = new ValueCell<int>(total_hit_points);
            
            Is_Alive_Cell = Current_Hit_Points_Cell.Select(x => x > 0);
        }

        private readonly UnitModel _unit;
        private readonly ValueCell<int> _current_hit_points_cell;
    }
}
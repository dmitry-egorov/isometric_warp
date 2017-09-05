using Lanski.Reactive;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class MHealth
    {
        public readonly int TotalHitPoints;
        public ICell<int> Current_Hit_Points_Cell => _current_hit_points_cell;
        public ICell<bool> Is_Alive_Cell { get; }
        internal bool Is_Alive => Is_Alive_Cell.Value;
        internal bool Is_Dead => !Is_Alive;

        public static MHealth From(UnitType type, MUnit unit) => new MHealth(type.GetHitPointsAmount(), unit);

        public void Take(DamageDescription damage)
        {
            _current_hit_points_cell.Value -= damage.Amount;
        }

        private MHealth(int total_hit_points, MUnit unit)
        {
            TotalHitPoints = total_hit_points;
            _unit = unit;
            _current_hit_points_cell = new ValueCell<int>(total_hit_points);
            
            Is_Alive_Cell = Current_Hit_Points_Cell.Select(x => x > 0);
        }

        private readonly MUnit _unit;
        private readonly ValueCell<int> _current_hit_points_cell;
    }
}
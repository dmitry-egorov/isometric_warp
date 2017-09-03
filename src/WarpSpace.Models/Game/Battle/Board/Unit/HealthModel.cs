using Lanski.Reactive;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit.Weapon;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class HealthModel
    {
        public readonly int TotalHitPoints;
        public ICell<int> Current_Hit_Points_Cell => _current_hit_points_cell;
        public ICell<bool> Is_Alive_Cell { get; }
        public bool Is_Alive => Is_Alive_Cell.Value;

        public static HealthModel From(UnitType type, DestructorModel destructor) => new HealthModel(type.GetHitPointsAmount(), destructor);

        public void Take(DamageDescription damage)
        {
            _current_hit_points_cell.Value -= damage.Amount;
            
            if(!Is_Alive) _destructor.Destroy();
        }

        private HealthModel(int total_hit_points, DestructorModel destructor)
        {
            TotalHitPoints = total_hit_points;
            _destructor = destructor;
            _current_hit_points_cell = new ValueCell<int>(total_hit_points);
            
            Is_Alive_Cell = Current_Hit_Points_Cell.Select(x => x > 0);
        }

        private readonly DestructorModel _destructor;
        private readonly ValueCell<int> _current_hit_points_cell;
    }
}
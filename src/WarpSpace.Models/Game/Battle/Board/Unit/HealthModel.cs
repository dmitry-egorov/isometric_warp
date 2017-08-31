using Lanski.Reactive;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit.Weapon;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class HealthModel
    {
        private readonly ValueCell<int> _currentHitPointsCell;

        public readonly int TotalHitPoints;
        public ICell<int> Current_Hit_Points_Cell => _currentHitPointsCell;
        public ICell<bool> Is_Alive_Cell { get; }
        public bool Is_Alive => Is_Alive_Cell.Value;

        public static HealthModel From(UnitType type) => new HealthModel(type.GetHitPointsAmount());

        private HealthModel(int totalHitPoints)
        {
            TotalHitPoints = totalHitPoints;
            _currentHitPointsCell = new ValueCell<int>(totalHitPoints);
            
            Is_Alive_Cell = Current_Hit_Points_Cell.Select(x => x > 0);
        }

        public void Take(DamageDescription damage)
        {
            _currentHitPointsCell.Value -= damage.Amount;
            Debug.Log($"An arrow to the knee! Amount: {damage.Amount}, now: {_currentHitPointsCell.Value}");
        }
    }
}
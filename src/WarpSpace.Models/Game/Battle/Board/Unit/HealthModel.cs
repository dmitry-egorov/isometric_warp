using Lanski.Reactive;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit.Weapon;

namespace WarpSpace.Models.Game.Battle.Board.Unit
{
    public class HealthModel
    {
        private readonly ValueCell<int> _currentHitPointsCell;

        public readonly int TotalHitPoints;
        public ICell<int> CurrentHitPoints => _currentHitPointsCell;
        public ICell<bool> IsAliveCell { get; }

        public static HealthModel From(UnitType type) => new HealthModel(type.GetHitPointsAmount());

        private HealthModel(int totalHitPoints)
        {
            TotalHitPoints = totalHitPoints;
            _currentHitPointsCell = new ValueCell<int>(totalHitPoints);
            
            IsAliveCell = CurrentHitPoints.Select(x => x > 0);
        }

        public bool Take(DamageDescription damage)
        {
            _currentHitPointsCell.Value -= damage.Amount;
            Debug.Log($"An arrow to the knee! Amount: {damage.Amount}, now: {_currentHitPointsCell.Value}");
            return true;
        }
    }
}
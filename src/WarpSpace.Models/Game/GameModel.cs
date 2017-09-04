using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Models.Game
{
    public class GameModel
    {
        private readonly BoardDescription _boardDescription;

        private readonly ValueCell<Slot<BattleModel>> _currentBattle;
        public ICell<Slot<BattleModel>> Current_Battle => _currentBattle;
        public ICell<Slot<PlayerModel>> Current_Player { get; }

        public GameModel(BoardDescription boardDescription)
        {
            _boardDescription = boardDescription;
            _currentBattle = ValueCellExtensions.Empty<BattleModel>();
            Current_Player = _currentBattle.Select(b => b.Select(x => x.Player));
        }

        public void Start()
        {
            RestartBattle();
        }
        
        public void RestartBattle()
        {
            var battle = BattleFactory.From(_boardDescription, this);

            _currentBattle.Value = battle.As_a_Slot();
            
            battle.Start();
        }
    }
}
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle;

namespace WarpSpace.Models.Game
{
    public class GameModel
    {
        private readonly BoardDescription _boardDescription;

        private readonly ValueCell<Slot<BattleModel>> _currentBattle;
        public ICell<Slot<BattleModel>> CurrentBattle => _currentBattle;

        public GameModel(BoardDescription boardDescription)
        {
            _boardDescription = boardDescription;
            _currentBattle = new ValueCell<Slot<BattleModel>>(null);
        }

        public void Start()
        {
            RestartBattle();
        }
        
        public void RestartBattle()
        {
            var battle = BattleFactory.From(_boardDescription, this);

            _currentBattle.Value = battle;
            
            battle.Start();
        }
    }
}
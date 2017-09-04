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
            _currentBattle = ValueCellEx.Empty<BattleModel>();
            Current_Player = _currentBattle.Select(b => b.Select(x => x.Player));

            Current_Battle
                .SelectMany(b => b.Select(x => x.Stream_Of_Exits).Value_Or_Empty())
                .Subscribe(_ => Restart_Battle());
        }

        public void Start()
        {
            Restart_Battle();
        }

        private void Restart_Battle()
        {
            var battle = new BattleModel(_boardDescription);

            _currentBattle.Value = battle.As_a_Slot();
            
            battle.Start();
        }
    }
}
using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Models.Game
{
    public class MGame
    {
        private readonly BoardDescription _boardDescription;

        private readonly ValueCell<Possible<MBattle>> _currentBattle;
        public ICell<Possible<MBattle>> Current_Battle => _currentBattle;
        public ICell<Possible<MPlayer>> Current_Player { get; }

        public MGame(BoardDescription boardDescription)
        {
            _boardDescription = boardDescription;
            _currentBattle = ValueCellEx.Empty<MBattle>();
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
            var battle = new MBattle(_boardDescription);

            _currentBattle.s_Value = battle;
            
            battle.Start();
        }
    }
}
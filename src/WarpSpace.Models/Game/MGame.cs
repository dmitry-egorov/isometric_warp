using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Models.Game
{
    public class MGame
    {
        public MGame(BoardDescription the_board_description)
        {
            this.the_board_description = the_board_description;
            
            s_signal_guard = new SignalGuard();
            s_player = new MPlayer(it.s_signal_guard);
            
            s_battles_cell = GuardedCell.Empty<MBattle>(it.s_signal_guard);

            it.s_battles_cell
                .SelectMany(b => b.Select(x => x.s_Stream_Of_Exits).Value_Or_Empty())
                .Subscribe(_ => restarts_the_battle());
        }
        
        public ICell<Possible<MBattle>> s_Battles_Cell => it.s_battles_cell;
        public MPlayer s_Player => it.s_player;

        public void Starts() => it.restarts_the_battle();

        private void restarts_the_battle()
        {
            using (it.s_signal_guard.Holds_All_Events())
            {
                it.s_Player.Resets_the_Selection();
            
                var battle = new MBattle(the_board_description, it.s_signal_guard);
                it.s_battles_cell.s_Value = battle;
            
                battle.Start();                
            }
        }

        
        private MGame it => this;
        private readonly SignalGuard s_signal_guard;
        private readonly BoardDescription the_board_description;
        private readonly GuardedCell<Possible<MBattle>> s_battles_cell;
        private readonly MPlayer s_player;
    }
}
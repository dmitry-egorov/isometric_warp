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
            its_board_description = the_board_description;
            its_signal_guard = new SignalGuard();
            its_player = new MPlayer(its_signal_guard);
            
            its_battles_cell = it_create_its_battles_cell();

            it_wires_the_stream_of_exits();
            
            GuardedCell<Possible<MBattle>> it_create_its_battles_cell() => GuardedCell.Empty<MBattle>(its_signal_guard);

            void it_wires_the_stream_of_exits() => 
                its_battles_cell
                .SelectMany(b => b.Select(x => x.s_Stream_Of_Exits).Value_Or_Empty())
                .Subscribe(_ => it_restarts_the_battle())
            ;
        }

        public ICell<Possible<MBattle>> s_Battles_Cell => its_battles_cell;
        public MPlayer s_Player => its_player;

        public void Starts() => it_restarts_the_battle();

        private void it_restarts_the_battle()
        {
            using (its_signal_guard.Holds_All_Events())
            {
                this.s_Player.Resets_the_Selection();
            
                var the_battle = new MBattle(its_board_description, its_signal_guard);
                its_battle = the_battle;
            
                the_battle.Starts();                
            }
        }

        private Possible<MBattle> its_battle
        {
            get => its_battles_cell.s_Value;
            set => its_battles_cell.s_Value = value;
        }

        private readonly BoardDescription its_board_description;
        private readonly SignalGuard its_signal_guard;
        private readonly GuardedCell<Possible<MBattle>> its_battles_cell;
        private readonly MPlayer its_player;
    }
}
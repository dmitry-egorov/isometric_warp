using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Models.Game
{
    public class MGame
    {
        public MGame(DBoard the_board_description, DUnit the_mothership_description, MFaction the_players_faction)
        {
            its_board_description = the_board_description;
            its_mothership_description = the_mothership_description;
            its_signal_guard = new SignalGuard();
            its_player = new MPlayer(this, the_players_faction, its_signal_guard);
            its_battles_cell = GuardedCell.Empty<MBattle>(its_signal_guard);
        }

        public ICell<Possible<MBattle>> s_Battles_Cell => its_battles_cell;
        public MPlayer s_Player => its_player;

        public void Starts_a_New_Battle() => it_restarts_the_battle();
        public MBattle must_have_a_Battle() => its_possible_battle.must_have_a_Value();

        private void it_restarts_the_battle()
        {
            using (its_signal_guard.Holds_All_Events())
            {
                its_player.Resets_the_Selection();
            
                var the_battle = new MBattle(its_board_description, its_mothership_description, its_signal_guard, this);
                its_possible_battle = the_battle;
            
                the_battle.Starts();
            }
        }

        private Possible<MBattle> its_possible_battle
        {
            get => its_battles_cell.s_Value;
            set => its_battles_cell.s_Value = value;
        }

        private readonly DBoard its_board_description;
        private readonly SignalGuard its_signal_guard;
        private readonly GuardedCell<Possible<MBattle>> its_battles_cell;
        private readonly MPlayer its_player;
        private readonly DUnit its_mothership_description;
    }
}
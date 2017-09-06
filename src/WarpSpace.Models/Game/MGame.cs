using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Models.Game
{
    public class MGame
    {
        public ICell<Possible<MBattle>> s_Battles_Cell => the_battles_cell;
        public ICell<Possible<MPlayer>> s_Players_cell => the_players_cell;

        public MGame(BoardDescription the_board_description)
        {
            this.the_board_description = the_board_description;
            the_events_guard = new EventsGuard();
            the_battles_cell = new GuardedCell<Possible<MBattle>>(Possible.Empty<MBattle>(), the_events_guard);
            the_players_cell = the_battles_cell.Select(b => b.Select(x => x.Player));

            s_Battles_Cell
                .SelectMany(b => b.Select(x => x.Stream_Of_Exits).Value_Or_Empty())
                .Subscribe(_ => Restart_Battle());
        }

        public void Start()
        {
            Restart_Battle();
        }

        private void Restart_Battle()
        {
            var battle = new MBattle(the_board_description, the_events_guard);

            the_battles_cell.s_Value = battle;
            
            battle.Start();
        }
        
        private readonly EventsGuard the_events_guard;
        private readonly BoardDescription the_board_description;
        private readonly GuardedCell<Possible<MBattle>> the_battles_cell;
        private readonly ICell<Possible<MPlayer>> the_players_cell;
    }
}
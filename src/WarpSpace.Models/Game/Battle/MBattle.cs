using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Models.Game.Battle
{
    public class MBattle
    {
        public readonly MBoard Board;
        public readonly MPlayer Player;

        public IStream<TheVoid> Stream_Of_Exits => Board.Stream_Of_Exits;

        public MBattle(BoardDescription board_description, EventsGuard the_events_guard)
        {
            Board = new MBoard(board_description, the_events_guard);
            Player = new MPlayer(the_events_guard);
        }

        public void Start()
        {
            Board.Warp_In_the_Mothership();
        }
    }
}
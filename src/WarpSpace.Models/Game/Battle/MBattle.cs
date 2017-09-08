using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board;

namespace WarpSpace.Models.Game.Battle
{
    public class MBattle
    {
        public readonly MBoard Board;

        public IStream<TheVoid> s_Stream_Of_Exits => Board.s_Stream_Of_Exits;

        public MBattle(BoardDescription board_description, SignalGuard the_signal_guard)
        {
            Board = new MBoard(board_description, the_signal_guard);
        }

        public void Start() => Board.Warps_In_the_Mothership();
        public void Ends_the_Turn() => Board.Ends_the_Turn();
    }
}
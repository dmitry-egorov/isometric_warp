using Lanski.Reactive;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board;

namespace WarpSpace.Models.Game.Battle
{
    public class MBattle
    {
        public MBattle(BoardDescription board_description, SignalGuard the_signal_guard)
        {
            its_board = new MBoard(board_description, the_signal_guard);
        }
        
        public MBoard s_Board => its_board;
        public IStream<TheVoid> s_Stream_Of_Exits => s_Board.s_Stream_Of_Exits;

        public void Starts() => s_Board.Warps_In_the_Mothership();
        public void Ends_the_Turn() => s_Board.Ends_the_Turn();
        
        private readonly MBoard its_board;
    }
}
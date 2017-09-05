using Lanski.Reactive;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Models.Game.Battle
{
    public class MBattle
    {
        public readonly MBoard Board;
        public readonly MPlayer Player;

        public IStream<MothershipExited> Stream_Of_Exits => Board.Stream_Of_Exits;

        public MBattle(BoardDescription board_description)
        {
            Board = new MBoard(board_description);
            Player = new MPlayer();
        }

        public void Start()
        {
            Board.Warp_In_the_Mothership();
        }
    }
}
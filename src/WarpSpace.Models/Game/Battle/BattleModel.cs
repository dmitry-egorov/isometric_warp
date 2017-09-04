using Lanski.Reactive;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Models.Game.Battle
{
    public class BattleModel
    {
        public readonly BoardModel Board;
        public readonly PlayerModel Player;

        public IStream<MothershipExited> Stream_Of_Exits => Board.Stream_Of_Exits;

        public BattleModel(BoardDescription board_description)
        {
            Board = new BoardModel(board_description);
            Player = new PlayerModel();
        }

        public void Start()
        {
            Board.Warp_In_the_Mothership();
        }
    }
}
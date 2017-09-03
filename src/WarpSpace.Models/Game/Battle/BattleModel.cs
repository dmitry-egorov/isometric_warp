namespace WarpSpace.Models.Game.Battle
{
    public class BattleModel
    {
        public readonly Board.BoardModel Board;
        public readonly Player.PlayerModel Player;

        public BattleModel(Board.BoardModel board, Player.PlayerModel player)
        {
            Board = board;
            Player = player;
        }

        public void Start()
        {
            Board.Warp_In_the_Mothership();
        }
    }
}
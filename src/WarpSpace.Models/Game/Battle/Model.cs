namespace WarpSpace.Models.Game.Battle
{
    public class Model
    {
        public readonly Board.Model Board;
        public readonly Player.Model Player;

        public Model(Board.Model board, Player.Model player)
        {
            Board = board;
            Player = player;
        }

        public void Start()
        {
            Board.WrapInMothership();
        }
    }
}
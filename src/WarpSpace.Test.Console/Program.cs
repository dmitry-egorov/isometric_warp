using Lanski.Structures;
using WarpSpace.Common;
using WarpSpace.Game.Battle;
using WarpSpace.Models.Game;

using Con = System.Console;

namespace WarpSpace.Test.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Con.WriteLine("Start");
            var board = new BoardData { 
                Units = 
                "- - - - - - - -\n" +
                "- - - - - - - -\n" +
                "- - - - - - - -\n" +
                "- - - - - - - -\n" +
                "- - - - - - - -\n" +
                "- - - - - - - -\n" +
                "- - - - - - - -\n" +
                "- - - - - - - -",
                Tiles = 
                "L L L L L L L L\n" +
                "L L L L L L L L\n" +
                "L L L L L L L L\n" +
                "L L L L L L L L\n" +
                "L L L L L L L L\n" +
                "L L L L L L L L\n" +
                "L L L L L L L L\n" +
                "L L L L L L L L",
                Entrance = new Spacial2DData {Position = new Index2DData() {Row = 2, Column = 2}, Orientation = Direction2D.Down}, 
                Exit     = new Spacial2DData {Position = new Index2DData() {Row = 5, Column = 5}, Orientation = Direction2D.Up}, 
            };
            
            var desc = board.ToDescription();
            var game = new MGame(desc);
            
            game.Starts();

            var ms = game.s_Battle.must_have_a_Value().s_Board.Units[2];
            
            game.s_Player.Executes_a_Command_At(game.s_Battle.must_have_a_Value().s_Board.Tiles.Get(new Index2D(3, 2)));
        }
    }
}
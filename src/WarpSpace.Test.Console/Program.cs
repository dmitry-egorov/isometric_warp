using Lanski.Structures;
using WarpSpace.Common;
using WarpSpace.Game.Battle;
using WarpSpace.Game.Battle.BoardGenerators;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;

using Con = System.Console;

namespace WarpSpace.Test.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Con.WriteLine("Start");
            var board = new PredefinedBoard { 
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
            var the_game = new MGame(desc);
            
            the_game.Starts_a_New_Battle();

            Scenario(the_game);

            the_game.Starts_a_New_Battle();
            
            Scenario(the_game);
            
            Con.WriteLine("Finished");
        }

        private static void Scenario(MGame the_game)
        {
            var the_battle = the_game.must_have_a_Battle();
            var the_player = the_game.s_Player;

            var tiles = the_battle.s_Board.s_Tiles;

            var ms_tile = tiles[3, 2];
            the_player.Executes_a_Command_At(ms_tile);
            the_player.Toggles_the_Selected_Action_With(DUnitAction.Create.Deploy(0));

            var tank_tile = tiles[4, 2];
            the_player.Executes_a_Command_At(tank_tile);

            the_player.Executes_a_Command_At(tank_tile);
            the_player.Toggles_the_Selected_Action_With(DUnitAction.Create.Dock());
            the_player.Executes_a_Command_At(ms_tile);
        }
    }
}
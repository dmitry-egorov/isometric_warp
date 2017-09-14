using System.Collections.Generic;
using Lanski.Structures;
using WarpSpace.Common;
using WarpSpace.Game.Battle.BoardGenerators;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static WarpSpace.Models.Descriptions.LandscapeType;
using static WarpSpace.Models.Descriptions.Passability;
using static WarpSpace.Models.Descriptions.WeaponType;
using Con = System.Console;

namespace WarpSpace.Test.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Con.WriteLine("Start");
            var a_track     = new MChassisType(new Dictionary<LandscapeType, Passability> {{Flatland, Free}, {Hill, Penalty}, {Mountain, None}, {Water, None}});
            var a_hower_pad = new MChassisType(new Dictionary<LandscapeType, Passability> {{Flatland, Free}, {Hill, None},    {Mountain, None}, {Water, None}});
            
            var tank_type = new MUnitType(2, 3, 0, a_Cannon, a_track, new DStuff(10), Possible.Empty<DStuff>(), true, false, 'T');
            var mothership_type = new MUnitType(5, 2, 4, a_Missle, a_hower_pad, new DStuff(50), Possible.Empty<DStuff>(), false, true, 'M');

            var the_players_faction = new MFaction();
            var the_natives_faction = new MFaction();
            var tank = new DUnit(tank_type, Possible.Empty<DStuff>(), new List<Possible<DUnit>>(), the_players_faction).as_a_Possible();
            var the_mothership = new DUnit(mothership_type, Possible.Empty<DStuff>(), new[] {tank, tank}, the_players_faction);

            var board = new PredefinedBoard
            {
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
                Entrance = new Spacial2DData
                {
                    Position = new Index2DData() {Row = 2, Column = 2},
                    Orientation = Direction2D.Down
                },
                Exit = new Spacial2DData
                {
                    Position = new Index2DData() {Row = 5, Column = 5},
                    Orientation = Direction2D.Up
                },
            };

            var desc = board.s_Description_With(new [] {tank_type, mothership_type}, the_natives_faction, mothership_type.s_Chassis_Type);
            var the_game = new MGame(desc, the_mothership, the_players_faction);

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
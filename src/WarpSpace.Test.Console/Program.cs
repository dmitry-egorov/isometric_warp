using System.Collections.Generic;
using Lanski.Structures;
using WarpSpace.Common;
using WarpSpace.Game.Battle.BoardGenerators;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle.Board.Unit;
using static WarpSpace.Models.Descriptions.Passability;
using Con = System.Console;

namespace WarpSpace.Test.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Con.WriteLine("Start");
            var the_flatland = new MLandscapeType('L');
            var the_hill     = new MLandscapeType('H');
            var the_mountain = new MLandscapeType('M');
            var the_water    = new MLandscapeType('W');
            var the_lanscape_types = new[] { the_flatland, the_hill, the_mountain, the_water };
            
            var a_track     = new MChassisType(new Dictionary<MLandscapeType, Passability> {{the_flatland, Single_Move}, {the_hill, All_Moves}, {the_mountain, Unavailable}, {the_water, Unavailable}});
            var a_hower_pad = new MChassisType(new Dictionary<MLandscapeType, Passability> {{the_flatland, Single_Move}, {the_hill, Unavailable},    {the_mountain, Unavailable}, {the_water, Unavailable}});

            var a_missale_launcher = new MWeaponType("Missile Launcher", 2, new DDamage(1));
            var a_cannon = new MWeaponType("Cannon", 1, new DDamage(2));
            
            var tank_type = new MUnitType("Tank", 2, 3, 0, a_cannon, a_track, new DStuff(10), Possible.Empty<DStuff>(), true, false, 'T');
            var mothership_type = new MUnitType("Mothership", 5, 2, 4, a_missale_launcher, a_hower_pad, new DStuff(50), Possible.Empty<DStuff>(), false, true, 'M');
            var the_unit_types = new [] {tank_type, mothership_type};

            var the_players_faction = new MFaction();
            var the_natives_faction = new MFaction();
            var tank = new DUnit(tank_type, the_players_faction).as_a_Possible();
            var the_mothership = new DUnit(mothership_type, the_players_faction, Possible.Empty<DStuff>(), new[] {tank, tank});

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

            var desc = board.s_Description_With(the_unit_types, the_lanscape_types, the_natives_faction, mothership_type.s_Chassis_Type);
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
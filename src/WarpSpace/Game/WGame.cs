using Lanski.Reactive;
using Lanski.Structures;
using UnityEditor.Expose;
using UnityEngine;
using WarpSpace.Game.Battle.BoardGenerators;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.Settings;

namespace WarpSpace.Game
{
    public class WGame: MonoBehaviour
    {
        public BoardSettings Board;
        public UnitSettings Mothership;
        public FactionSettings PlayersFaction;
        public FactionSettings NativesFaction;
        
        [TextArea(8,8)] public string LastMapString;//For inspector
        
        public void Awake() => it_inits();
        public void Start() => this.Restarts();
        
        public bool has_a_Player(out MPlayer the_player) => its_players_cell.has_a_Value(out the_player);
        
        public ICell<Possible<MUnit>> s_Players_Selected_Units_Cell => its_players_selected_units_cell;
        public ICell<Possible<MPlayer.Selection>> s_Players_Selections_Cell => its_players_selections_cell;
        public ICell<Possible<MBattle>> s_Battles_Cell => its_battles_cell;
        public Possible<MPlayer> s_Possible_Player => its_players_cell.s_Value;
        public MPlayer s_Player => this.s_Possible_Player.must_have_a_Value();

        [ExposeMethodInEditor]
        public void Restarts()
        {
            it_inits();

            var the_game = it_creates_a_game();
            its_game_becomes(the_game);
            the_game.Starts_a_New_Battle();
        }

        private void it_inits()
        {
            if (it_is_initialized)
                return;
            it_is_initialized = true;
            
            its_games_cell = Cell.Empty<MGame>();
            its_players_cell = its_games_cell.Select(the_possible_game => the_possible_game.Select(the_game => the_game.s_Player));
            its_battles_cell = its_games_cell.SelectMany(gc => gc.Select_Cell_Or_Single_Default(g => g.s_Battles_Cell));
            its_players_selected_units_cell = its_players_cell.SelectMany(pp => pp.Select(p => p.s_Selected_Units_Cell).Cell_Or_Single_Default());
            its_players_selections_cell = its_players_cell.SelectMany(pp => pp.Select(p => p.s_Selections_Cell).Cell_Or_Single_Default());
        }

        private MGame it_creates_a_game()
        {
            var the_players_faction = FactionSettings.s_Model_Of(PlayersFaction);
            var the_natives_faction = FactionSettings.s_Model_Of(NativesFaction);
            var the_landscape_types = LandscapeTypeSettings.s_All_Models;
            var the_unit_types = UnitTypeSettings.s_All_Models;
            
            var the_mothership = Mothership.s_Description_With(the_players_faction);
            var the_board_desc = Board.s_Description_With(the_unit_types, the_landscape_types, the_natives_faction, the_mothership.s_Type.s_Chassis_Type);

            _lastMap = the_board_desc;
            LastMapString = the_board_desc.Display();
            
            return new MGame(the_board_desc, the_mothership, the_players_faction);
        }

        private void its_game_becomes(MGame the_game) => its_games_cell.s_Value = the_game;

        private bool it_is_initialized;
        private Cell<Possible<MGame>> its_games_cell;
        private ICell<Possible<MPlayer>> its_players_cell;
        private ICell<Possible<MBattle>> its_battles_cell;
        private ICell<Possible<MUnit>> its_players_selected_units_cell;
        private ICell<Possible<MPlayer.Selection>> its_players_selections_cell;

        private DBoard _lastMap;//For inspector
    }
}

/*
        1373467341
        
M M M H H H H M
M M H L H H L H
H H W W L L L H
L H W W M H H M
L W H L H H M M
L L L L H M M H
M H H W W W L L
M H W W W W W W

M M M M H H M M
M W W M H L M M
M W W L L L H M
H H L L M W H L
L L H M W W W W
W W H M W W M H
H L L H H H L H
L L M M M L L W
        */
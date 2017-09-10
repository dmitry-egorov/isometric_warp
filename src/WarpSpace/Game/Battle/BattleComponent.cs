using System;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEditor.Expose;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.Services;

namespace WarpSpace.Game.Battle
{
    public class BattleComponent: MonoBehaviour
    {
        public OptionalPredefinedBoardsSettings PredefinedBoards;
        [TextArea(8,8)] public string LastMapString;//For inspector
        
        private void it_inits()
        {
            if (it_is_initialized)
                return;
            it_is_initialized = true;
            
            its_board = FindObjectOfType<BoardComponent>();
            
            its_games_cell = Cell.Empty<MGame>();
            its_players_cell = its_games_cell.Select(the_possible_game => the_possible_game.Select(the_game => the_game.s_Player));
            its_battles_cell = its_games_cell.SelectMany(gc => gc.Select_Cell_Or_Single_Default(g => g.s_Battles_Cell));
            its_players_selected_units_cell = its_players_cell.SelectMany(pp => pp.Select(p => p.s_Selected_Units_Cell).Cell_Or_Single_Default());
            its_players_selections_cell = its_players_cell.SelectMany(pp => pp.Select(p => p.s_Selections_Cell).Cell_Or_Single_Default());
            
            wires_the_board();
        
            void wires_the_board()
            {
                its_battles_cell.Subscribe(possible_battle =>
                {
                    if (possible_battle.Doesnt_Have_a_Value(out var the_battle))
                        return;
                    
                    its_board.Inits(the_battle.s_Board, this.s_possible_game.must_have_a_Value().s_Player);
                });
            }
        }

        public ICell<Possible<MUnit>> s_Players_Selected_Units_Cell => its_players_selected_units_cell;
        public ICell<Possible<MPlayer.Selection>> s_Players_Selections_Cell => its_players_selections_cell;
        public ICell<Possible<MPlayer>> s_Players_Cell => its_players_cell;
        public ICell<Possible<MBattle>> s_Battles_Cell => its_battles_cell;
        
        public bool has_a_Selection(out MPlayer.Selection the_selection) => its_players_selections_cell.has_a_Value(out the_selection);
        public bool has_a_Battle(out MBattle the_battle) => its_battles_cell.has_a_Value(out the_battle);
        public bool has_a_Player(out MPlayer the_player) => its_players_cell.has_a_Value(out the_player);
        
        public void Awake()
        {
            it_inits();
        }
        
        public void Start()
        {
            Restarts();
        }

        [ExposeMethodInEditor]
        public void Restarts()
        {
            it_inits();

            var the_game = it_creates_a_game();
            its_game_becomes(the_game);
            the_game.Starts();
        }

        private MGame it_creates_a_game()
        {
            var the_board_description = it_creates_a_board_description();

            _lastMap = the_board_description;
            LastMapString = the_board_description.Display();
            
            return new MGame(the_board_description);
                
            BoardDescription it_creates_a_board_description() => 
                PredefinedBoards
                    .Nullable
                    .Select(x => x.GetPredefinedBoard())
                    .Value_Or(it_generates_random_map);

            BoardDescription it_generates_random_map() => new RandomBoardGenerator(new UnityRandom()).GenerateRandomMap();
        }

        private void its_game_becomes(MGame the_game) => its_games_cell.s_Value = the_game;

        private Possible<MGame> s_possible_game => its_games_cell.s_Value;

        private bool it_is_initialized;
        private Cell<Possible<MGame>> its_games_cell;
        private ICell<Possible<MPlayer>> its_players_cell;
        private ICell<Possible<MBattle>> its_battles_cell;
        private ICell<Possible<MUnit>> its_players_selected_units_cell;
        private ICell<Possible<MPlayer.Selection>> its_players_selections_cell;
        private BoardComponent its_board;

        private BoardDescription _lastMap;//For inspector

        [Serializable]
        public class OptionalPredefinedBoardsSettings: Optional<PredefinedBoardsSettings>{}
            
        [Serializable]
        public struct BoardData
        {
            [TextArea(8,8)] public string Tiles;
            [TextArea(8,8)] public string Units;
            public Spacial2DData Entrance;
            public Spacial2DData Exit;
        }

        [Serializable]
        public struct PredefinedBoardsSettings
        {
            public BoardData[] Boards;
            public int Index;
    
            public BoardDescription GetPredefinedBoard()
            {
                return Boards[Index].ToDescription();
            }
        }

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
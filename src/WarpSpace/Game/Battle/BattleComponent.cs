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

        public ICell<Possible<MUnit>> s_Selected_Units_Cell => it.s_selected_units_cell;
        public ICell<Possible<MPlayer.Selection>> s_Selections_Cell => it.s_selections_cell;
        public ICell<Possible<MPlayer>> s_Players_Cell => it.s_players_cell;
        public ICell<Possible<MBattle>> s_Battles_Cell => it.s_battles_cell;
        
        public bool has_a_Battle(out MBattle the_battle) => it.s_battles_cell.has_a_Value(out the_battle);
        public bool has_a_Player(out MPlayer the_player) => it.s_players_cell.has_a_Value(out the_player);

        public void Awake()
        {
            inits();
        }
        
        public void Start()
        {
            Restarts();
        }

        [ExposeMethodInEditor]
        public void Restarts()
        {
            it.inits();

            var the_game = it.creates_a_game();
            it.s_game_becomes(the_game);
            the_game.Starts();
        }

        private MGame creates_a_game()
        {
            var boardDescription = the_board_description();

            _lastMap = boardDescription;
            LastMapString = boardDescription.Display();
                
            return new MGame(boardDescription);
                
            BoardDescription the_board_description() => 
                PredefinedBoards
                    .Nullable
                    .Select(x => x.GetPredefinedBoard())
                    .Value_Or(it_generates_random_map);

            BoardDescription it_generates_random_map() => new RandomBoardGenerator(new UnityRandom()).GenerateRandomMap();
        }

        private void inits()
        {
            if (_initialized)
                return;
            _initialized = true;
            
            the_board = FindObjectOfType<BoardComponent>();
            
            it.s_games_cell = Cell.Empty<MGame>();
            it.s_players_cell = it.s_games_cell.Select(the_possible_game => the_possible_game.Select(the_game => the_game.s_Player));
            it.s_battles_cell = it.s_games_cell.SelectMany(gc => gc.Select_Cell_Or_Single_Default(g => g.s_Battles_Cell));
            it.s_selected_units_cell = it.s_players_cell.SelectMany(pp => pp.Select(p => p.s_Selected_Units_Cell).Cell_Or_Single_Default());
            it.s_selections_cell = it.s_players_cell.SelectMany(pp => pp.Select(p => p.s_Selections_Cell).Cell_Or_Single_Default());
            
            wires_the_board();
        }
        
        void wires_the_board()
        {
            it.s_Battles_Cell.Subscribe(battle_ref =>
            {
                if (battle_ref.Doesnt_Have_a_Value(out var battle))
                    return;
                    
                the_board.Init(battle.Board, it.s_possible_game.must_have_a_Value().s_Player);
            });
        }

        private void s_game_becomes(MGame the_game) => it.s_games_cell.s_Value = the_game;

        private Possible<MGame> s_possible_game => it.s_games_cell.s_Value; 

        private BattleComponent it => this;

        private bool _initialized;
        private Cell<Possible<MGame>> s_games_cell;
        private ICell<Possible<MPlayer>> s_players_cell;
        private ICell<Possible<MBattle>> s_battles_cell;
        private ICell<Possible<MUnit>> s_selected_units_cell;
        private ICell<Possible<MPlayer.Selection>> s_selections_cell;
        private BoardComponent the_board;

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
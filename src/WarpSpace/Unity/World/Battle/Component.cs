using System;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEditor.Expose;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Unity.World.Battle
{
    public class Component: MonoBehaviour
    {
        public OptionalPredefinedBoardsSettings PredefinedBoards;
        [TextArea(8,8)] public string LastMapString;//For inspector
        private BoardDescription _lastMap;//For inspector

        private bool _initialized;
        private ValueCell<Slot<GameModel>> _gameCell;
        public ICell<Slot<GameModel>> Game_Cell => _gameCell;
        public ICell<Slot<PlayerModel>> Player_Cell { get; private set; }
        public ICell<Slot<BattleModel>> Battle_Cell { get; private set; }

        void Awake()
        {
            Init();
        }
        
        void Start()
        {
            Restart();
        }

        [ExposeMethodInEditor]
        public void Restart()
        {
            Init();

            var board = FindObjectOfType<Board.Component>();//TODO: create from prefab

            var game = Create_the_Game();
            Wire_Board_Component_to_the_Game();
            _gameCell.Value = game.As_a_Slot();

            Start_the_Game();

            GameModel Create_the_Game()
            {
                var boardDescription = GetBoardDescription();

                _lastMap = boardDescription;
                LastMapString = boardDescription.Display();
                
                return new GameModel(boardDescription);
                
                BoardDescription GetBoardDescription() => 
                    PredefinedBoards
                        .Nullable
                        .Select(x => x.GetPredefinedBoard())
                        .GetValueOr(GenerateRandomMap);

                BoardDescription GenerateRandomMap() => new RandomBoardGenerator(new UnityRandom()).GenerateRandomMap();
            }
            
            void Wire_Board_Component_to_the_Game()
            {
                game.Current_Battle.Subscribe(battle_ref =>
                {
                    if (battle_ref.Doesnt_Have(out var battle))
                        return;
                    
                    board.Init(battle.Board, battle.Player);
                });
            }

            void Start_the_Game()
            {
                game.Start();
            }
        }

        private void Init()
        {
            if (_initialized)
                return;
            _initialized = true;
            
            _gameCell = ValueCellExtensions.Empty<GameModel>();
            Player_Cell = Game_Cell.SelectMany(gc => gc.Select(g => g.Current_Player).Cell_Or_Single_Default());
            Battle_Cell = Game_Cell.SelectMany(gc => gc.Select(g => g.Current_Battle).Cell_Or_Single_Default());
        }

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
using System;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEditor.Expose;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Descriptions;

namespace WarpSpace.Unity.World.Battle
{
    public class Component: MonoBehaviour
    {
        public OptionalPredefinedBoardsSettings PredefinedBoards;
        [TextArea(8,8)] public string LastMap;//For inspector

        void Start()
        {
            Restart();
        }

        [ExposeMethodInEditor]
        public void Restart()
        {
            var board = FindObjectOfType<Board.Component>();//TODO: create from prefab

            var game = Create_the_Game();
            Wire_Board_Component_to_the_Game();
            Start_the_Game();

            Models.Game.Model Create_the_Game()
            {
                var boardDescription = GetBoardDescription();

                LastMap = boardDescription.Display();

                return new Models.Game.Model(boardDescription);
                
                BoardDescription GetBoardDescription() => 
                    PredefinedBoards
                        .Nullable
                        .Select(x => x.GetPredefinedBoard())
                        .GetValueOr(RandomBoardGenerator.GenerateRandomMap);
            }
            
            void Wire_Board_Component_to_the_Game()
            {
                game.CurrentBattle.Subscribe(b =>
                {
                    if (b == null)
                        return;
                    
                    board.Init(b.Board, b.Player);
                });
            }

            void Start_the_Game()
            {
                game.Start();
            }
        }

        [Serializable]
        public class OptionalPredefinedBoardsSettings: Optional<PredefinedBoardsSettings>{}
            
        [Serializable]
        public struct BoardData
        {
            [TextArea(8,8)] public string Tiles;
            public Spacial2DData Entrance;
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
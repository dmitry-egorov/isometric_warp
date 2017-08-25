using System;
using System.Linq;
using Lanski.Behaviours;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEditor.Expose;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.World.Board.Tile.Descriptions;

namespace WarpSpace.World
{
    public class Factory: EditorInitializable
    {
        public Board.Settings BoardSettings;
        public OptionalPredefinedBoardsSettings PredefinedBoards;
        [TextArea(8,8)]public string LastMap;//For inspector

        private Board.Component _board;
        private Player.Component _player;

        protected override void Init()
        {
            _board = FindObjectOfType<Board.Component>();
            _player = FindObjectOfType<Player.Component>();
        }

        protected override void OnParameterChanged()
        {
            Restart();
        }

        [ExposeMethodInEditor]
        public void Restart()
        {
            CreateBoard();
            WarpPlayer();
            
            void CreateBoard()
            {
                var map = CreateBoardSpec();
                
                LastMap = map.Display();

                InitBoard();

                BoardDescription CreateBoardSpec() => 
                    PredefinedBoards
                    .Nullable
                    .Select(x => x.GetPredefinedBoard())
                    .GetValueOr(RandomMapGenerator.GenerateRandomMap);

                void InitBoard()
                {
                    var specs = ConvertToComponentSpec();
                    _board.Init(specs);
                }

                Board.ComponentSpec ConvertToComponentSpec() => 
                    new Board.ComponentSpecConverter(BoardSettings)
                    .GenerateComponentSpec(map, _player);
            }

            void WarpPlayer()
            {
                _board.Entrance.WarpPlayer();
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
                return Boards[Index].ToSpec();
            }
    
            public bool Equals(PredefinedBoardsSettings other)
            {
                return Boards.SequenceEqual(other.Boards) && Index == other.Index;
            }
    
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is PredefinedBoardsSettings && Equals((PredefinedBoardsSettings) obj);
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
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Unity.World.Battle.Board
{
    public class Component : MonoBehaviour
    {
        public OwnSettings Settings;

        public void Init(BoardModel board, PlayerModel player)
        {
            this.DestroyChildren();

            var tile_components = CreateTiles();
            var components_map = new Dictionary<UnitModel, Unit.Component>();

            UnitCreationWiring.Wire(board, tile_components, components_map, Settings);
            TileHighlightsWiring.Wire(player, board, tile_components);
            OutlinesWiring.Wire(player, components_map);

            Tile.Component[,] CreateTiles() => 
                board.Tiles.Map((tile, index) =>
                {
                    var n = board.Tiles.GetFitNeighbours(index).Map(t => t.Landscape.Type);
                    return Tile.Component.Create(Settings.TilePrefab, transform, tile, n, board.Tiles.GetDimensions(), player);
                })
            ;
        }

        [Serializable]
        public struct OwnSettings
        {
            public GameObject TilePrefab;
            public GameObject MothershipPrefab;
            public GameObject TankPrefab;
        }
    }
}
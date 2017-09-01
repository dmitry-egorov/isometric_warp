using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Player;
using WarpSpace.Unity.World.Battle.Unit;

namespace WarpSpace.Unity.World.Battle.Board
{
    public class Component : MonoBehaviour
    {
        public GameObject TilePrefab;
        public GameObject UnitPrefab;
        
        private RepeatAllStream<UnitComponent> _stream_of_created_units;
        public IStream<UnitComponent> Stream_Of_Created_Units => _stream_of_created_units;

        public void Init(BoardModel board, PlayerModel player)
        {
            gameObject.DestroyChildren();
            
            _stream_of_created_units = new RepeatAllStream<UnitComponent>();

            var tile_components = CreateTiles();

            UnitCreationWiring.Wire(board, tile_components, UnitPrefab, player, _stream_of_created_units);
            TileHighlightsWiring.Wire(player, board, tile_components);

            Tile.Component[,] CreateTiles() => 
                board.Tiles.Map((tile, index) =>
                {
                    var n = board.Tiles.GetFitNeighbours(index).Map(t => t.Landscape.Type);
                    return Tile.Component.Create(TilePrefab, transform, tile, n, board.Tiles.GetDimensions(), player);
                })
            ;
        }
    }
}
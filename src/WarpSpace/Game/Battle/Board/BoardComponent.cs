using Lanski.Reactive;
using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Game.Battle.Unit;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Board
{
    public class BoardComponent : MonoBehaviour
    {
        public GameObject TilePrefab;
        public GameObject UnitPrefab;
        public GameObject Limbo;
        
        private RepeatAllStream<UnitComponent> _stream_of_created_units;
        public IStream<UnitComponent> Stream_Of_Created_Units => _stream_of_created_units;

        public void Init(MBoard board, MPlayer player)
        {
            gameObject.DestroyChildren();
            
            _stream_of_created_units = new RepeatAllStream<UnitComponent>();

            var tile_components = CreateTiles();

            UnitCreationWiring.Wire(board, tile_components, UnitPrefab, Limbo, player, _stream_of_created_units);
            TileHighlightsWiring.Wire(player, board, tile_components);

            Tile.TileComponent[,] CreateTiles() => 
                board.Tiles.Map((tile, index) =>
                {
                    var n = board.Tiles.GetFitNeighbours(index).Map(t => t.Type_Of_the_Landscape());
                    return Tile.TileComponent.Create(TilePrefab, transform, tile, n, board.Tiles.GetDimensions(), player);
                })
            ;
        }
    }
}
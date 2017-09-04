using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Unity.World.Battle.Board.Tile
{
    public class TileComponent : MonoBehaviour
    {
        public UnitSlot.Component UnitSlot { get; private set; }
        public HighlightElement Highlight { get; private set; }
        
        public static TileComponent Create(GameObject prefab, Transform parent, TileModel tile, FullNeighbourhood2D<LandscapeType> neighbourhood, Dimensions2D dimensions, PlayerModel player)
        {
            var component = Instantiate(prefab, parent).GetComponent<TileComponent>();
            component.Init(tile, neighbourhood, dimensions, player);
            return component;
        }

        private void Init(TileModel tile, FullNeighbourhood2D<LandscapeType> neighbourhood, Dimensions2D dimensions, PlayerModel player)
        {
            _tile = tile;
            var position = tile.Position;

            transform.localPosition = GetPosition(position, dimensions);
            name = $"Tile ({position.Column}, {position.Row})";

            var landscape = GetComponentInChildren<Landscape.Component>();
            var water = GetComponentInChildren<Water.Component>();
            var structureSlot = GetComponentInChildren<StructureSlot.Component>();
            var playerActionsDetector = GetComponentInChildren<PlayerActionSource.Component>();

            UnitSlot = GetComponentInChildren<UnitSlot.Component>();
            Highlight = new HighlightElement(player, tile, landscape);

            landscape.Init(position, neighbourhood);
            water.Init(position, neighbourhood);
            structureSlot.Init(tile);

            playerActionsDetector.Init();
            
            WirePlayerActionsDetector();

            void WirePlayerActionsDetector() => 
                playerActionsDetector
                .Actions
                .Subscribe(() => player.Execute_Command_At(tile));
        }

        private static Vector3 GetPosition(Index2D i, Dimensions2D dimensions) => new Vector3(i.Column - dimensions.Columns * 0.5f, 0, dimensions.Rows * 0.5f - i.Row);
        
        private TileModel _tile;//For debug
    }
}
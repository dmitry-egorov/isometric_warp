using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Game.Battle.Tile
{
    public class TileComponent : MonoBehaviour
    {
        public UnitSlot UnitSlot { get; private set; }
        public HighlightElement Highlight { get; private set; }
        
        public static TileComponent Create(GameObject prefab, Transform parent, MTile tile, FullNeighbourhood2D<LandscapeType> neighbourhood, Dimensions2D dimensions, MPlayer player)
        {
            var component = Instantiate(prefab, parent).GetComponent<TileComponent>();
            component.Init(tile, neighbourhood, dimensions, player);
            return component;
        }

        private void Init(MTile tile, FullNeighbourhood2D<LandscapeType> neighbourhood, Dimensions2D dimensions, MPlayer player)
        {
            _tile = tile;
            var position = tile.Position;

            transform.localPosition = GetPosition(position, dimensions);
            name = $"Tile ({position.Column}, {position.Row})";

            var landscape = GetComponentInChildren<Landscape>();
            var water = GetComponentInChildren<Water>();
            var structureSlot = GetComponentInChildren<StructureSlot>();
            var playerActionsDetector = GetComponentInChildren<PlayerActionSource>();

            UnitSlot = GetComponentInChildren<UnitSlot>();
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
        
        private MTile _tile;//For debug
    }
}
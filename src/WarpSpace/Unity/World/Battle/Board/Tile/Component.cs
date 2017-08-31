using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Player;

namespace WarpSpace.Unity.World.Battle.Board.Tile
{
    public class Component : MonoBehaviour
    {
        public Landscape.Component Landscape { get; private set; }
        public UnitSlot.Component UnitSlot { get; private set; }
        public HighlightElement Highlight { get; private set; }
        
        public static Component Create(GameObject prefab, Transform parent, TileModel tile, FullNeighbourhood2D<LandscapeType> neighbourhood, Dimensions2D dimensions, PlayerModel player)
        {
            var component = Instantiate(prefab, parent).GetComponent<Component>();
            component.Init(tile, neighbourhood, dimensions, player);
            return component;
        }

        private void Init(TileModel tile, FullNeighbourhood2D<LandscapeType> neighbourhood, Dimensions2D dimensions, PlayerModel player)
        {
            var position = tile.Position;

            transform.localPosition = GetPosition(position, dimensions);
            name = $"Tile ({position.Column}, {position.Row})";

            Landscape = GetComponentInChildren<Landscape.Component>();
            var water = GetComponentInChildren<Water.Component>();
            var structureSlot = GetComponentInChildren<StructureSlot.Component>();
            UnitSlot = GetComponentInChildren<UnitSlot.Component>();
            var playerActionsDetector = GetComponentInChildren<PlayerActionSource.Component>();
            Highlight = new HighlightElement(player, tile, Landscape);

            Landscape.Init(position, neighbourhood);
            water.Init(position, neighbourhood);
            
            if (tile.Structure_Cell.Has_a_Value(out var structure))
                structureSlot.Init(structure.Description);

            playerActionsDetector.Init();
            
            WirePlayerActionsDetector();

            void WirePlayerActionsDetector()
            {
                playerActionsDetector
                    .Actions
                    .Subscribe(() => player.Execute_Command_At(tile));
            }
        }

        private static Vector3 GetPosition(Index2D i, Dimensions2D dimensions)
        {
            return new Vector3(i.Column - dimensions.Columns * 0.5f, 0, dimensions.Rows * 0.5f - i.Row);
        }
    }
}
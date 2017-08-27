using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Unity.World.Battle.Board.Tile
{
    public class Component : MonoBehaviour
    {
        public Model Model { get; private set; }
        public Landscape.Component Landscape { get; private set; }
        public UnitSlot.Component UnitSlot { get; private set; }
        
        public static Component Create(GameObject prefab, Transform parent, Model tile, FullNeighbourhood2D<LandscapeType> neighbourhood, Dimensions2D dimensions, Models.Game.Battle.Player.Model player)
        {
            var component = Instantiate(prefab, parent).GetComponent<Component>();
            component.Init(tile, neighbourhood, dimensions, player);
            return component;
        }

        private void Init(Model tile, FullNeighbourhood2D<LandscapeType> neighbourhood, Dimensions2D dimensions, Models.Game.Battle.Player.Model player)
        {
            var position = tile.Position;

            transform.localPosition = GetPosition(position, dimensions);
            name = $"Tile ({position.Column}, {position.Row})";

            Model = tile;
            Landscape = GetComponentInChildren<Landscape.Component>();
            var water = GetComponentInChildren<Water.Component>();
            var structureSlot = GetComponentInChildren<StructureSlot.Component>();
            UnitSlot = GetComponentInChildren<UnitSlot.Component>();
            var playerActionsDetector = GetComponentInChildren<PlayerActionSource.Component>();

            Landscape.Init(position, neighbourhood);
            water.Init(position, neighbourhood);
            structureSlot.Init(tile.Structure);
            playerActionsDetector.Init();
            
            WirePlayerActionsDetector();

            void WirePlayerActionsDetector()
            {
                playerActionsDetector
                    .Actions
                    .Subscribe(() => player.ExecuteActionAt(tile));
            }
        }

        private static Vector3 GetPosition(Index2D i, Dimensions2D dimensions)
        {
            return new Vector3(i.Column - dimensions.Columns * 0.5f, 0, dimensions.Rows * 0.5f - i.Row);
        }
        
        
    }
}
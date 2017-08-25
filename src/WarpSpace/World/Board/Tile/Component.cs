using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.World.Board.Tile
{
    public class Component : MonoBehaviour
    {
        public Landscape.Component Landscape { get; private set; }
        public StructureSlot.Component StructureSlot { get; private set; }
        public Water.Component Water { get; private set; }
        public UnitSlot.Component UnitSlot { get; private set; }
        public Commander.Component Commander { get; private set; }
        public Index2D Position { get; private set; }
        public AdjacentRef<Component> AdjacentTiles { get; private set; }
        public Highlight.Component Highlight { get; private set; }

        public static Component Create(GameObject prefab, Transform parent)
        {
            return Instantiate(prefab, parent).GetComponent<Component>();
        }

        public void Init(ComponentSpec spec, AdjacentRef<Component> adjacent)
        {
            transform.localPosition = spec.SpacePosition;
            name = spec.Name;

            Position = spec.Position;
            AdjacentTiles = adjacent;
            
            Landscape = GetComponentInChildren<Landscape.Component>();
            Water = GetComponentInChildren<Water.Component>();
            StructureSlot = GetComponentInChildren<StructureSlot.Component>();
            UnitSlot = GetComponentInChildren<UnitSlot.Component>();
            Commander = GetComponentInChildren<Commander.Component>();
            Highlight = GetComponentInChildren<Highlight.Component>();
        
            Landscape.Init(spec.Landscape);
            Water.Init(spec.Water);
            StructureSlot.Init(spec.Structure);
            Commander.Init(spec.Player, this);
            Highlight.Init(Landscape);
        }

        public bool IsAdjacentTo(Component destination)
        {
            return Position.IsAdjacentTo(destination.Position);
        }

        public Direction2D GetDirectionTo(Component tile)
        {
            return Position.DirectionTo(tile.Position);
        }
    }
}
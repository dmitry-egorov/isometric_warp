using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;

namespace WarpSpace.World.Board
{
    public class Component : MonoBehaviour
    {
        private Entrance _entrance;

        public Entrance Entrance => _entrance;

        public void Init(ComponentSpec specs)
        {
            this.DestroyChildren();
            
            var components = CreateTiles(specs);
            
            _entrance = CreateEntrance(components, specs.Entrance); 
        }

        private Tile.Component[,] CreateTiles(ComponentSpec specs)
        {
            var tiles = specs.Tiles.Map(_ => Tile.Component.Create(specs.TilePrefab, transform));

            foreach (var i in tiles.EnumerateIndex())
            {
                var spec = specs.Tiles.Get(i);
                var adjacent = tiles.GetAdjacent(i);
                tiles.Get(i).Init(spec, adjacent);
            }

            return tiles;
        }

        private static Entrance CreateEntrance(Tile.Component[,] components, EntranceSpec entranceSpecs)
        {
            var tile = components.Get(entranceSpecs.Spacial.Position);
            var mothershipPrefab = entranceSpecs.MothershipPrefab;
            var orientation = entranceSpecs.Spacial.Orientation;

            return new Entrance(tile, mothershipPrefab, orientation);
        }
    }
}
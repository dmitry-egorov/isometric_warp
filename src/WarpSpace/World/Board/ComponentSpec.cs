using UnityEngine;

namespace WarpSpace.World.Board
{
    public struct ComponentSpec
    {
        public readonly GameObject TilePrefab;
        public readonly Tile.ComponentSpec[,] Tiles;
        public readonly EntranceSpec Entrance;
        
        public ComponentSpec(GameObject tilePrefab, Tile.ComponentSpec[,] tiles, EntranceSpec entrance)
        {
            TilePrefab = tilePrefab;
            Tiles = tiles;
            Entrance = entrance;
        }

    }
}
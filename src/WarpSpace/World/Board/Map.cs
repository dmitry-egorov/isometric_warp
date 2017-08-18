using UnityEngine;

namespace WarpSpace.World.Board
{
    public class Map : MonoBehaviour
    {
        public Tile[,] Tiles { get; private set; }
        
        public void SetTiles(Tile[,] tiles)
        {
            Tiles = tiles;
        }
    }
}
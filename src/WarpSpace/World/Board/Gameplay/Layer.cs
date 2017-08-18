using Lanski.Structures;
using UnityEditor.Expose;
using UnityEngine;

namespace WarpSpace.World.Board.Gameplay
{
    public class Layer: MonoBehaviour
    {
        Tile _entrance;
        Tile[,] _tiles;

        public void Init(Tile[,] tiles, Tile entrance)
        {
            _entrance = entrance;
            _tiles = tiles;
        }

        public void SpawnPlayer()
        {
            
        }
    }
}
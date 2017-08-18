using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Planet.Tiles;

namespace WarpSpace.World.Board.Water
{
    public class Factory : MonoBehaviour
    {
        [SerializeField] GameObject _tilePrefab;

        public void RecreateTiles(Map map)
        {
            this.DestroyChildren();
            CreateTiles(map.Tiles);
        }

        private void CreateTiles(Board.Tile[,] board)
        {
            var d = board.GetDimensions();
            
            foreach (var ei in board.EnumerateWithIndex())//TODO: don't create if there's no water tiles around
            {
                var i = ei.index;
                var tile = TileCreation.CreateTile(_tilePrefab, i, d, transform);

                tile.GetComponent<Tile>().Init(TileCreation.GetDirection(i), TileCreation.GetMirror(i));
            }
        }
    }
}
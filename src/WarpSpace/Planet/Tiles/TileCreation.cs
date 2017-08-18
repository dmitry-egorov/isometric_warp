using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.Planet.Tiles
{
    public static class TileCreation
    {
        public static Vector3 GetPosition(Index2D i, Dimensions2D dimensions)
        {
            return new Vector3(i.Column - dimensions.Columns * 0.5f, 0, dimensions.Rows * 0.5f - i.Row);
        }

        public static Direction2D GetDirection(Index2D i)
        {
            return (Direction2D)((i.Column + i.Row) % 4);
        }

        public static bool GetMirror(Index2D i)
        {
            return i.Column % 2 == 1;
        }

        public static GameObject CreateTile(GameObject tilePrefab, Index2D i, Dimensions2D d, Transform parent)
        {
            var tile = Object.Instantiate(tilePrefab, parent);

            tile.transform.localPosition = GetPosition(i, d);
            tile.name = $"Tile ({i.Column}, {i.Row})";
            return tile;
        }
    }
}
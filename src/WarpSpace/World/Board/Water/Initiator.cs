using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;

namespace WarpSpace.World.Board.Water
{
    public static class Initiator
    {
        public static void InitElement(Tile tile)
        {
            var index = tile.Index;
            
            tile
                .WaterElement
                .Init(TileCreation.GetDirection(index), TileCreation.GetMirror(index));
        }
    }
}
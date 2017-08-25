using System;
using UnityEngine;

namespace WarpSpace.World.Board.Tile.Landscape
{
    [Serializable]
    public struct TileSettings
    {
        public float OwnHeight;
        public float SameTypeHeight;
        public float SameTypeHeightCross;
        public float Falloff;
        public Mesh[] Meshes;
    }
}
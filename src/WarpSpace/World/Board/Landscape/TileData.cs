using System;
using UnityEngine;

namespace WarpSpace.World.Board.Landscape
{
    [Serializable]
    public struct TileData
    {
        public float OwnHeight;
        public float SameTypeHeight;
        public float SameTypeHeightCross;
        public float Falloff;
        public Mesh[] TerrainMeshes;
    }
}
using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.World.Board
{
    public static class TileCreationHelper
    {
        public static Direction2D GetOrientation(Index2D i)
        {
            return (Direction2D)((i.Column + i.Row) % 4);
        }

        public static bool GetMirror(Index2D i)
        {
            return i.Column % 2 == 1;
        }

        public static Mesh SelectMesh(Index2D index, Mesh[] meshes)
        {
            return meshes[index.Row % meshes.Length];
        }
    }
}
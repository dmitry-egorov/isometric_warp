using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.World.Board.Tile.Water
{
    public struct ComponentSpec
    {
        public readonly Quaternion Rotation;
        public readonly Vector3 Scale;
        public readonly Mesh Mesh;

        public ComponentSpec(Quaternion rotation, Vector3 scale, Mesh mesh)
        {
            Rotation = rotation;
            Scale = scale;
            Mesh = mesh;
        }
    }
}
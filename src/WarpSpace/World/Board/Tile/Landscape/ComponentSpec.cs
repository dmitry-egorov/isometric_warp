using UnityEngine;
using WarpSpace.World.Board.Tile.Descriptions;

namespace WarpSpace.World.Board.Tile.Landscape
{
    public struct ComponentSpec
    {
        public readonly Mesh Mesh;
        public readonly LandscapeType Type;

        public ComponentSpec(Mesh mesh, LandscapeType type)
        {
            Mesh = mesh;
            Type = type;
        }

    }
}
using System.Linq;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.World.Board.Tile.Descriptions;

namespace WarpSpace.World.Board.Tile.Water
{
    public class SpecGenerator
    {
        private readonly Mesh[] _meshes;

        public SpecGenerator(Mesh[] meshes)
        {
            _meshes = meshes;
        }

        public ComponentSpec? Init(Index2D index, FullNeighbourhood2D<LandscapeType> neighbors)
        {
            var anyWaterAround = neighbors.Elements.Enumerate().Any(x => x == LandscapeType.Water);
            
            return anyWaterAround 
                ? new ComponentSpec(GetRotation(index), GetScale(index), SelectMesh(index))
                : default(ComponentSpec?);
        }

        private Mesh SelectMesh(Index2D index)
        {
            return TileCreationHelper.SelectMesh(index, _meshes);
        }

        private static Vector3 GetScale(Index2D index)
        {
            return TileCreationHelper.GetMirror(index) ? new Vector3(-1, 1, 1) : Vector3.one;
        }

        private static Quaternion GetRotation(Index2D index)
        {
            return TileCreationHelper.GetOrientation(index).ToRotation();
        }
    }
}
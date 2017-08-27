using System;
using System.Linq;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Descriptions;

namespace WarpSpace.Unity.World.Battle.Board.Tile.Water
{
    public class Component : MonoBehaviour
    {
        public OwnSettings Settings;
        public MeshFilter MeshFilter;
        
        public void Init(Index2D index, FullNeighbourhood2D<LandscapeType> neighbors)
        {
            var anyWaterAround = neighbors.Elements.Enumerate().Any(x => x == LandscapeType.Water);

            if (anyWaterAround)
            {
                MeshFilter.sharedMesh = SelectMesh();
                transform.localRotation = GetRotation();
                transform.localScale = GetScale();
            }

            Mesh SelectMesh()
            {
                return Settings.Meshes.SelectBy(index);
            }

            Vector3 GetScale()
            {
                return TileCreationHelper.GetMirror(index) ? new Vector3(-1, 1, 1) : Vector3.one;
            }

            Quaternion GetRotation()
            {
                return TileCreationHelper.GetOrientation(index).ToRotation();
            }
        }
        
        [Serializable]
        public struct OwnSettings
        {
            public Mesh[] Meshes;
        }
    }
}
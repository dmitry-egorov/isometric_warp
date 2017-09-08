using System;
using System.Linq;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Game.Battle.Tile
{
    public class Water : MonoBehaviour
    {
        public OwnSettings Settings;
        public MeshFilter MeshFilter;
        
        public void Init(Index2D index, FullNeighbourhood2D<LandscapeType> neighbors)
        {
            var anyWaterAround = neighbors.Elements.Enumerate().Any(x => x == LandscapeType.Water);

            if (anyWaterAround)
            {
                MeshFilter.sharedMesh = SelectMesh();
                
                var its_transform = transform;
                its_transform.localRotation = GetRotation();
                its_transform.localScale = GetScale();
            }

            Mesh SelectMesh() => Settings.Meshes.SelectBy(index);
            Vector3 GetScale() => TileCreationHelper.GetMirror(index) ? new Vector3(-1, 1, 1) : Vector3.one;
            Quaternion GetRotation() => TileHelper.GetOrientation(index).To_Rotation();
        }
        
        [Serializable]
        public struct OwnSettings
        {
            public Mesh[] Meshes;
        }
    }
}
using System;
using System.Linq;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Game.Battle.Tile
{
    public class Water : MonoBehaviour
    {
        public OwnSettings Settings;
        public MeshFilter MeshFilter;
        
        public void Start()
        {
            var the_tile = GetComponentInParent<WTile>().s_Tile_Model;
            var the_position = the_tile.s_Position;
            var anyWaterAround = the_tile.s_Neighbors.Elements.Enumerate().Any(x => x.s_Landscape_Type == LandscapeType.Water);

            if (anyWaterAround)
            {
                MeshFilter.sharedMesh = SelectMesh();
                
                var its_transform = transform;
                its_transform.localRotation = GetRotation();
                its_transform.localScale = GetScale();
            }

            Mesh SelectMesh() => Settings.Meshes.SelectBy(the_position);
            Vector3 GetScale() => TileCreationHelper.GetMirror(the_position) ? new Vector3(-1, 1, 1) : Vector3.one;
            Quaternion GetRotation() => TileHelper.GetOrientation(the_position).To_Rotation();
        }
        
        [Serializable]
        public struct OwnSettings
        {
            public Mesh[] Meshes;
        }
    }
}
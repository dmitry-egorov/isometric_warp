using System;
using System.Linq;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Tile
{
    public class WWater : MonoBehaviour
    {
        public OwnSettings Settings;
        
        public void Start()
        {
            var the_tile = GetComponentInParent<WTile>().s_Tile_Model;
            var the_mesh_filter = GetComponentInChildren<MeshFilter>();
            var the_position = the_tile.s_Position;
            var anyWaterAround = the_tile.s_Neighbors.Elements.Enumerate().Any(x => LandscapeTypeSettings.Of(x.s_Landscape_Type).World.IsWaterLayer);

            if (anyWaterAround)
            {
                the_mesh_filter.sharedMesh = SelectMesh();
                
                var its_transform = transform;
                its_transform.localRotation = GetRotation();
                its_transform.localScale = GetScale();
            }

            Mesh SelectMesh() => Settings.Meshes.SelectBy(the_position);
            Vector3 GetScale() => TileCreationHelper.GetMirror(the_position) ? new Vector3(-1, 1, 1) : Vector3.one;
            Quaternion GetRotation() => TileHelper.GetOrientation(the_position).s_Rotation();
        }
        
        [Serializable]
        public struct OwnSettings
        {
            public Mesh[] Meshes;
        }
    }
}
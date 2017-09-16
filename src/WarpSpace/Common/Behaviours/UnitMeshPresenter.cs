using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Settings;

namespace WarpSpace.Common.Behaviours
{
    public class UnitMeshPresenter
    {
        public UnitMeshPresenter(MeshRenderer the_mesh_renderer)
        {
            its_mesh_renderer = the_mesh_renderer;
            its_mesh_filter = the_mesh_renderer.GetComponent<MeshFilter>();
        }

        public void Presents(MUnit the_unit) => Presents(the_unit.s_Type, the_unit.s_Faction);
        public void Presents(MUnitType type, MFaction faction)
        {
            its_mesh_filter.sharedMesh = UnitTypeSettings.Of(type).Mesh;
            its_mesh_renderer.sharedMaterial = FactionSettings.Of(faction).Material;
            
            its_mesh_renderer.gameObject.Shows();
        }

        public void Hides() => its_mesh_renderer.gameObject.Hides();

        private readonly MeshFilter its_mesh_filter;
        private readonly MeshRenderer its_mesh_renderer;
    }
}
using UnityEngine;
using WarpSpace.Models.Game;
using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class UnitMesh : MonoBehaviour
    {
        public void Present(MUnitType type, MFaction faction)
        {
            it_inits();

            its_mesh_filter.sharedMesh = UnitTypeSettings.Of(type).Mesh;
            its_mesh_renderer.sharedMaterial = FactionSettings.Of(faction).Material;
            
            Show();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void it_inits()
        {
            if (its_initialized)
                return;
            its_initialized = true;
            
            its_mesh_filter = GetComponent<MeshFilter>();
            its_mesh_renderer = GetComponent<MeshRenderer>();
        }

        private MeshFilter its_mesh_filter;
        private MeshRenderer its_mesh_renderer;
        private bool its_initialized;
    }
}
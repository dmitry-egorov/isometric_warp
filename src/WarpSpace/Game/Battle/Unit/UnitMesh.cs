using UnityEngine;
using WarpSpace.Models.Descriptions;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class UnitMesh : MonoBehaviour
    {
        public void Present(UnitType type, Faction faction)
        {
            it_inits();

            its_mesh_filter.sharedMesh = its_settings_holder.For(type).Mesh;
            its_mesh_renderer.sharedMaterial = its_settings_holder.For(faction).Material;
            
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
            
            its_settings_holder = FindObjectOfType<UnitSettingsHolder>();
            its_mesh_filter = GetComponent<MeshFilter>();
            its_mesh_renderer = GetComponent<MeshRenderer>();
        }

        private UnitSettingsHolder its_settings_holder;
        private MeshFilter its_mesh_filter;
        private MeshRenderer its_mesh_renderer;
        private bool its_initialized;
    }
}
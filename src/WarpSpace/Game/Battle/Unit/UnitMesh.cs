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
            inits();

            s_mesh_filter.sharedMesh = it.s_settings_holder.For(type).Mesh;
            s_mesh_renderer.sharedMaterial = it.s_settings_holder.For(faction).Material;
            
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

        private void inits()
        {
            if (it.s_initialized)
                return;
            it.s_initialized = true;
            
            it.s_settings_holder = FindObjectOfType<UnitSettingsHolder>();
            it.s_mesh_filter = GetComponent<MeshFilter>();
            it.s_mesh_renderer = GetComponent<MeshRenderer>();
        }

        private UnitMesh it => this;
        private UnitSettingsHolder s_settings_holder;
        private MeshFilter s_mesh_filter;
        private MeshRenderer s_mesh_renderer;
        private bool s_initialized;
    }
}
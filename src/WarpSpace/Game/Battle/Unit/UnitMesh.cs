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

            its_mesh_filter.sharedMesh = its_unit_type_settings_holder.s_Settings_Of(type).Mesh;
            its_mesh_renderer.sharedMaterial = its_faction_settings_holder.s_Settings_Of(faction).Material;
            
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
            
            its_unit_type_settings_holder = FindObjectOfType<UnitTypeSettingsHolder>();
            its_faction_settings_holder = FindObjectOfType<FactionSettingsHolder>();
            its_mesh_filter = GetComponent<MeshFilter>();
            its_mesh_renderer = GetComponent<MeshRenderer>();
        }

        private UnitTypeSettingsHolder its_unit_type_settings_holder;
        private FactionSettingsHolder its_faction_settings_holder;
        private MeshFilter its_mesh_filter;
        private MeshRenderer its_mesh_renderer;
        private bool its_initialized;
    }
}
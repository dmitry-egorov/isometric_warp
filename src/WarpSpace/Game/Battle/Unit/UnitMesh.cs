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
            var settings_holder = FindObjectOfType<UnitSettingsHolder>();

            GetComponent<MeshFilter>().sharedMesh = settings_holder.Get_Settings_For(type).Mesh;
            GetComponent<MeshRenderer>().sharedMaterial = settings_holder.Get_Settings_For(faction).Material;
            
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
    }
}
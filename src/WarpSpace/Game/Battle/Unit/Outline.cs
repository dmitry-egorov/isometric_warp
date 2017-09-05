using UnityEngine;
using WarpSpace.Models.Descriptions;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Unit
{
    [RequireComponent(typeof(OutlineMeshBuilder))]
    public class Outline : MonoBehaviour
    {
        public void Enable() => gameObject.SetActive(true);
        public void Disable() => gameObject.SetActive(false);

        public void Init(UnitType type)
        {
            var settings_holder = FindObjectOfType<UnitSettingsHolder>();
            var mesh = settings_holder.Get_Settings_For(type).Mesh;
            
            GetComponent<OutlineMeshBuilder>().Build_From(mesh);
            Disable();
        }
    }
}
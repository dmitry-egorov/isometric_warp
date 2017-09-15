using UnityEngine;

namespace WarpSpace.Settings
{
    [CreateAssetMenu(fileName = "UIButtonSettings", menuName = "Custom/UI/Button Settings", order = 1)]
    public class UIButtonSettings : ScriptableObject
    {
        public Material DisabledMaterial;
        
        public Material NormalMaterial;
        public Material PressedMaterial;
        
        public Material NormalToggledMaterial;
        public Material PressedToggledMaterial;
    }
}
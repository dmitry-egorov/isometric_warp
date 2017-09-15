using System;
using UnityEngine;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Settings
{
    [CreateAssetMenu(fileName = "MLandscapeType", menuName = "Custom/Landscape Type", order = 1)]
    public class LandscapeTypeSettings : SettingsObject<LandscapeTypeSettings, MLandscapeType>
    {
        public WorldSettings World;
        public char SerializationSymbol;

        protected override MLandscapeType Creates_a_Model() => new MLandscapeType(SerializationSymbol);

        [Serializable]
        public struct WorldSettings
        {
            public float OwnHeight;
            public float SameTypeHeight;
            public float SameTypeHeightCross;
            public float Falloff;
            public Mesh[] Meshes;
            public bool IsWaterLayer;
        }
    }
}
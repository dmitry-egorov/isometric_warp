using System;
using System.Linq;
using UnityEngine;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Settings
{
    [CreateAssetMenu(fileName = "ChassisType", menuName = "Custom/Chassis Type", order = 1)]
    public class ChassisTypeSettings : SettingsObject<MChassisType>
    {
        public LandscapeToPassability[] PassableLandscapeTypes; 
        public override MChassisType Creates_a_Model() => new MChassisType(LandscapeTypeEx.All.ToDictionary(t => t, t => PassableLandscapeTypes.FirstOrDefault(pt => pt.LandscapeType == t).Passability));

        [Serializable]
        public struct LandscapeToPassability
        {
            public LandscapeType LandscapeType;
            public Passability Passability;
        }
    }
}
using System;
using System.Linq;
using UnityEngine;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Settings
{
    [CreateAssetMenu(fileName = "ChassisType", menuName = "Custom/Chassis Type", order = 1)]
    public class ChassisTypeSettings : SettingsObject<ChassisTypeSettings, MChassisType>
    {
        public LandscapeToPassability[] PassableLandscapeTypes;

        protected override MChassisType Creates_a_Model() => 
            new MChassisType(LandscapeTypeSettings.s_All_Models.ToDictionary(t => t, t => PassableLandscapeTypes.FirstOrDefault(pt => LandscapeTypeSettings.s_Model_Of(pt.LandscapeType) == t).Passability))
        ;

        [Serializable]
        public struct LandscapeToPassability
        {
            public LandscapeTypeSettings LandscapeType;
            public Passability Passability;
        }
    }
}
using UnityEngine;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Settings
{
    [CreateAssetMenu(fileName = "WeaponType", menuName = "Custom/Weapon Type", order = 1)]
    public class WeaponTypeSettings : SettingsObject<WeaponTypeSettings, MWeaponType>
    {
        public WeaponTypeModelSettings Model;

        protected override MWeaponType Creates_a_Model() => Model.s_Description;
    }
}
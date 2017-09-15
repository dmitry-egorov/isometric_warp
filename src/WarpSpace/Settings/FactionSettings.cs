using Lanski.Structures;
using UnityEngine;
using WarpSpace.Models.Game;

namespace WarpSpace.Settings
{
    [CreateAssetMenu(fileName = "Faction", menuName = "Custom/Faction", order = 1)]
    public class FactionSettings: SettingsObject<FactionSettings, MFaction>
    {
        public Material Material;

        protected override MFaction Creates_a_Model() => new MFaction();
    }
}
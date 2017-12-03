using System;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Settings
{
    [Serializable]
    public struct WeaponTypeModelSettings
    {
        public int MaxUses;
        public DamageSettings Damage;


        [Serializable]
        public struct DamageSettings
        {
            public int Amount;
        
            public DDamage s_Model => new DDamage(Amount);
        }
    }
}
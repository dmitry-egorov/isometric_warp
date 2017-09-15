using System;
using WarpSpace.Models.Descriptions;

namespace WarpSpace.Settings
{
    [Serializable]
    public struct WeaponTypeModelSettings
    {
        public int MaxUses;
        public DamageSettings Damage;

        public MWeaponType s_Description => new MWeaponType(MaxUses, Damage.s_Model);
        
        
        [Serializable]
        public struct DamageSettings
        {
            public int Amount;
        
            public DDamage s_Model => new DDamage(Amount);
        }
    }
}
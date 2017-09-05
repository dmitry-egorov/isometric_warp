using System;

namespace WarpSpace.Models.Descriptions
{
    public static class WeaponTypeExtensions
    {
        public static DamageDescription Get_Damage_Description(this WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Missle:
                    return new DamageDescription(1, DamageType.Missle);
                case WeaponType.Cannon:
                    return new DamageDescription(2, DamageType.Cannon);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
    }
    
    public enum WeaponType
    {
        Missle,
        Cannon
    }
}
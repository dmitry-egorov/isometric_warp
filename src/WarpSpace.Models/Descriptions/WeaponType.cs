using System;

namespace WarpSpace.Models.Descriptions
{
    public static class WeaponTypeExtensions
    {
        public static int s_Max_Uses(this WeaponType type)
        {
            switch (type)
            {
                case WeaponType.a_Missle:
                    return 2;
                case WeaponType.a_Cannon:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static DDamage s_Damage_Description(this WeaponType type)
        {
            switch (type)
            {
                case WeaponType.a_Missle:
                    return new DDamage(1);
                case WeaponType.a_Cannon:
                    return new DDamage(2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
    }
    
    public enum WeaponType
    {
        a_Missle,
        a_Cannon
    }
}
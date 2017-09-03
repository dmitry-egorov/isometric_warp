using System;

namespace WarpSpace.Descriptions
{
    public static class UnitTypeExtensions
    {
        public static UnitType? ToUnitType(this char c)
        {
            switch (c)
            {
                case 'T':
                    return UnitType.Tank;
                default:
                    return null;
            }
        }

        public static int GetHitPointsAmount(this UnitType type)
        {
            switch (type)
            {
                case UnitType.Mothership:
                    return 5;
                case UnitType.Tank:
                    return 2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static WeaponType Get_Weapon_Type(this UnitType type)
        {
            switch (type)
            {
                case UnitType.Mothership:
                    return WeaponType.Missle;
                case UnitType.Tank:
                    return WeaponType.Cannon;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static ChassisType Get_Chassis_Type(this UnitType type)
        {
            switch (type)
            {
                case UnitType.Mothership:
                    return ChassisType.Hower;
                case UnitType.Tank:
                    return ChassisType.Tread;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public enum UnitType
    {
        Mothership,
        Tank
    }
}
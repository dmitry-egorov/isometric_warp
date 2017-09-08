using System;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public static class UnitTypeExtensions
    {
        public static Possible<UnitType> ToUnitType(this char c)
        {
            switch (c)
            {
                case 'T':
                    return UnitType.a_Tank;
                default:
                    return Possible.Empty<UnitType>();
            }
        }

        public static int s_Hit_Points(this UnitType type)
        {
            switch (type)
            {
                case UnitType.a_Mothership:
                    return 5;
                case UnitType.a_Tank:
                    return 2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static WeaponType Get_Weapon_Type(this UnitType type)
        {
            switch (type)
            {
                case UnitType.a_Mothership:
                    return WeaponType.a_Missle;
                case UnitType.a_Tank:
                    return WeaponType.a_Cannon;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static int s_Max_Moves(this UnitType type)
        {
            switch (type)
            {
                case UnitType.a_Mothership:
                    return 2;
                case UnitType.a_Tank:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static ChassisType s_Chassis_Type(this UnitType type)
        {
            switch (type)
            {
                case UnitType.a_Mothership:
                    return ChassisType.a_Hower_Pad;
                case UnitType.a_Tank:
                    return ChassisType.a_Track;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static bool Requires_a_Bay(this UnitType type, out int size)
        {
            switch (type)
            {
                case UnitType.a_Mothership:
                    size = 4;
                    return true;
                case UnitType.a_Tank:
                    size = 0;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public enum UnitType
    {
        a_Mothership,
        a_Tank
    }
}
using System;
using Lanski.Structures;
using static WarpSpace.Models.Descriptions.UnitType;

namespace WarpSpace.Models.Descriptions
{
    public static class UnitTypeExtensions
    {
        public static Possible<UnitType> s_Unit_Type(this char c)
        {
            switch (c)
            {
                case 'T':
                    return a_Tank;
                default:
                    return Possible.Empty<UnitType>();
            }
        }

        public static int s_Total_Hit_Points(this UnitType type)
        {
            switch (type)
            {
                case a_Mothership:
                    return 5;
                case a_Tank:
                    return 2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static WeaponType s_Weapon_Type(this UnitType type)
        {
            switch (type)
            {
                case a_Mothership:
                    return WeaponType.a_Missle;
                case a_Tank:
                    return WeaponType.a_Cannon;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static int s_Total_Moves(this UnitType type)
        {
            switch (type)
            {
                case a_Mothership:
                    return 2;
                case a_Tank:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public static ChassisType s_Chassis_Type(this UnitType type)
        {
            switch (type)
            {
                case a_Mothership:
                    return ChassisType.a_Hower_Pad;
                case a_Tank:
                    return ChassisType.a_Track;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static int s_Bay_Size(this UnitType type)
        {
            switch (type)
            {
                case a_Mothership:
                    return 4;
                case a_Tank:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static Possible<DStuff> s_Initial_Staff(this UnitType type)
        {
            switch (type)
            {
                case a_Mothership:
                    return Possible.Empty<DStuff>();
                case a_Tank:
                    return new DStuff(10);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static bool can_Dock(this UnitType the_type) => the_type != a_Mothership;
        public static bool can_Use_an_Exit(this UnitType the_type) => the_type == a_Mothership;
    }

    public enum UnitType
    {
        a_Mothership,
        a_Tank
    }
}
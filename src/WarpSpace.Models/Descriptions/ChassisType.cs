using System;

namespace WarpSpace.Models.Descriptions
{
//    public static class ChassisTypeExtensions
//    {
//        public static Passability s_Passability_Of(this ChassisType the_chassis_type, LandscapeType the_landscape_type)
//        {
//            switch (the_chassis_type)
//            {
//                case ChassisType.a_Hower_Pad:
//                    switch (the_landscape_type)
//                    {
//                        case LandscapeType.Mountain:
//                            return Passability.None;
//                        case LandscapeType.Hill:
//                            return Passability.None;
//                        case LandscapeType.Flatland:
//                            return Passability.Free;
//                        case LandscapeType.Water:
//                            return Passability.None;
//                        default:
//                            throw new ArgumentOutOfRangeException(nameof(the_landscape_type), the_landscape_type, null);
//                    }
//                case ChassisType.a_Track:
//                    switch (the_landscape_type)
//                    {
//                        case LandscapeType.Mountain:
//                            return Passability.None;
//                        case LandscapeType.Hill:
//                            return Passability.Penalty;
//                        case LandscapeType.Flatland:
//                            return Passability.Free;
//                        case LandscapeType.Water:
//                            return Passability.None;
//                        default:
//                            throw new ArgumentOutOfRangeException(nameof(the_landscape_type), the_landscape_type, null);
//                    }
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(the_chassis_type), the_chassis_type, null);
//            }
//        }
//
//        public static bool can_Pass(this ChassisType the_chassis_type, LandscapeType the_landscape_type) => the_chassis_type.s_Passability_Of(the_landscape_type) != Passability.None;
//    }
//    
//    public enum ChassisType
//    {
//        a_Hower_Pad,
//        a_Track
//    }
}
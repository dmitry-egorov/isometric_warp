using System;

namespace WarpSpace.Models.Descriptions
{
    public static class LandscapeTypeExtensions
    {
        private static readonly string _chars = "MHLW";
            
        public static char ToChar(this LandscapeType type)
        {
            return _chars[(int) type];
        }

        public static LandscapeType ToLandscapeType(this char c)
        {
            return (LandscapeType) _chars.IndexOf(c);
        }

        public static bool is_passable_with(this LandscapeType landscape, ChassisType chassisType)
        {
            return landscape.GetPassability(chassisType) != Passability.None;
        }
        
        public static Passability GetPassability(this LandscapeType type, ChassisType chassisType)
        {
            switch (chassisType)
            {
                case ChassisType.a_Hower_Pad:
                    switch (type)
                    {
                        case LandscapeType.Mountain:
                            return Passability.None;
                        case LandscapeType.Hill:
                            return Passability.None;
                        case LandscapeType.Flatland:
                            return Passability.Free;
                        case LandscapeType.Water:
                            return Passability.None;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                case ChassisType.a_Track:
                    switch (type)
                    {
                        case LandscapeType.Mountain:
                            return Passability.None;
                        case LandscapeType.Hill:
                            return Passability.Penalty;
                        case LandscapeType.Flatland:
                            return Passability.Free;
                        case LandscapeType.Water:
                            return Passability.None;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(chassisType), chassisType, null);
            }
            
        }
    }

    public enum Passability
    {
        None,
        Penalty,
        Free
    }

    public enum LandscapeType
    {
        Mountain,
        Hill,
        Flatland,
        Water
    }
}
using System;
using static WarpSpace.Descriptions.ChassisType;
using static WarpSpace.Descriptions.LandscapeType;
using static WarpSpace.Descriptions.Passability;

namespace WarpSpace.Descriptions
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

        public static bool IsPassableWith(this LandscapeType landscape, ChassisType chassisType)
        {
            return landscape.GetPassability(chassisType) != None;
        }
        
        public static Passability GetPassability(this LandscapeType type, ChassisType chassisType)
        {
            switch (chassisType)
            {
                case Hower:
                    switch (type)
                    {
                        case Mountain:
                            return None;
                        case Hill:
                            return None;
                        case Flatland:
                            return Free;
                        case Water:
                            return None;
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

    public static class LandscapeTypeEx
    {
        public static LandscapeType Random()
        {
            return (LandscapeType)UnityEngine.Random.Range(0, 4);
        }
    }
    
    public enum LandscapeType
    {
        Mountain,
        Hill,
        Flatland,
        Water
    }
}
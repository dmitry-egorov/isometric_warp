using System;

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

        public static bool IsPassableWith(this LandscapeType landscape, Chassis chassis)
        {
            return landscape.GetPassability(chassis) != Passability.None;
        }
        
        public static Passability GetPassability(this LandscapeType type, Chassis chassis)
        {
            switch (chassis)
            {
                case Chassis.Mothership:
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(chassis), chassis, null);
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
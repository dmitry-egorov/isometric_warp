using System;

namespace WarpSpace.World.Board
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

        public static bool IsPassable(this LandscapeType type)
        {
            return type.GetPassability() != Passability.None;
        }
        
        public static Passability GetPassability(this LandscapeType type)
        {
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
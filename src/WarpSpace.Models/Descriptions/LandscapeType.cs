using System;
using System.Collections.Generic;
using System.Linq;

namespace WarpSpace.Models.Descriptions
{
    public static class LandscapeTypeEx
    {
        public static IEnumerable<LandscapeType> All => Enum.GetValues(typeof(LandscapeType)).Cast<LandscapeType>();
    }
    
    public static class LandscapeTypeExtensions
    {
        private const string Chars = "MHLW";

        public static char s_Char(this LandscapeType type) => Chars[(int) type];

        public static LandscapeType s_Landscape_Type(this char c) => (LandscapeType) Chars.IndexOf(c);
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
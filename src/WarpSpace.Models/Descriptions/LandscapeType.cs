namespace WarpSpace.Models.Descriptions
{
    public static class LandscapeTypeExtensions
    {
        private const string Chars = "MHLW";

        public static char s_Char(this LandscapeType type)
        {
            return Chars[(int) type];
        }

        public static LandscapeType s_Landscape_Type(this char c)
        {
            return (LandscapeType) Chars.IndexOf(c);
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
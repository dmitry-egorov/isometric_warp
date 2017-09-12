namespace WarpSpace.Models.Descriptions
{
    public struct DTile
    {
        public readonly LandscapeType Type;
        public readonly DTileSite Initial_Site;

        public DTile(LandscapeType type, DTileSite initial_site)
        {
            Type = type;
            Initial_Site = initial_site;
        }
    }
}
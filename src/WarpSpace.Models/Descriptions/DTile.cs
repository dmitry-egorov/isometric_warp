namespace WarpSpace.Models.Descriptions
{
    public struct DTile
    {
        public readonly MLandscapeType Type;
        public readonly DTileSite Initial_Site;

        public DTile(MLandscapeType type, DTileSite initial_site)
        {
            Type = type;
            Initial_Site = initial_site;
        }
    }
}
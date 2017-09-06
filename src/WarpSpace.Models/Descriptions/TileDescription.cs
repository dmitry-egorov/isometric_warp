namespace WarpSpace.Models.Descriptions
{
    public struct TileDescription
    {
        public readonly LandscapeType Type;
        public readonly TileSiteDescription Initial_Site;

        public TileDescription(LandscapeType type, TileSiteDescription initial_site)
        {
            Type = type;
            Initial_Site = initial_site;
        }
    }
}
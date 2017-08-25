namespace WarpSpace.World.Board.Tile.Landscape.Extensions
{
    public static class Factory
    {
        public static Initiator ToInitiator(this Settings s)
        {
            return new Initiator(s.Mountain, s.Hill, s.Flatland, s.Water);
        }
    }
}
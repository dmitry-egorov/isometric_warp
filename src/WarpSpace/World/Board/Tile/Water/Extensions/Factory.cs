namespace WarpSpace.World.Board.Tile.Water.Extensions
{
    public static class Factory
    {
        public static SpecGenerator ToInitiator(this Settings structureSettings)
        {
            return new SpecGenerator(structureSettings.Meshes);
        }
    }
}
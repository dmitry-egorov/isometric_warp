namespace WarpSpace.World.Board.Tile.StructureSlot.Extensions
{
    public static class Factory
    {
        public static Initiator ToInitiator(this Settings structureSettings)
        {
            return new Initiator(structureSettings.EntrancePrefab);
        }
    }
}
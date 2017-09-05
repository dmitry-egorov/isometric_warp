namespace WarpSpace.Models.Game.Battle.Board.Structure
{
    public struct StructureDestroyed
    {
        private readonly MStructure _structure;

        public StructureDestroyed(MStructure structure)
        {
            _structure = structure;
        }
    }
}
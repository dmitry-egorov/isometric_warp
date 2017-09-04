namespace WarpSpace.Models.Game.Battle.Board.Structure
{
    public struct StructureDestroyed
    {
        private readonly StructureModel _structure;

        public StructureDestroyed(StructureModel structure)
        {
            _structure = structure;
        }
    }
}
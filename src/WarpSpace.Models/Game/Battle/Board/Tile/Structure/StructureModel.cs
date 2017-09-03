using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure
{
    public class StructureModel
    {
        public readonly StructureDescription Description;
        public readonly IInteractor Interactor;

        internal StructureModel(StructureDescription description, IInteractor interactor)
        {
            Description = description;
            Interactor = interactor;
        }
    }
}
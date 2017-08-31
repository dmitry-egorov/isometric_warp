using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure
{
    public class StructureModel
    {
        public readonly StructureDescription Description;
        public readonly IInteractor Interactor;
        
        public StructureModel(StructureDescription description, GameModel game, TileModel tile)
        {
            Description = description;
            Interactor = Factory.From(description.Type, game, tile);
        }
    }
}
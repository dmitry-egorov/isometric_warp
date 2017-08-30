using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    public interface IInteractor
    {
        bool CanBeInteractedBy(UnitModel unit);
        bool TryInteractBy(UnitModel unit);
    }
}
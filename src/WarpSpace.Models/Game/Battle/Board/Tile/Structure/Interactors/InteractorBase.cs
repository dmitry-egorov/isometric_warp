using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    public abstract class InteractorBase : IInteractor
    {
        public abstract bool CanBeInteractedBy(UnitModel unit);

        public bool TryInteractBy(UnitModel unit)
        {
            if (!CanBeInteractedBy(unit))
                return false;

            Interact(unit);
            return true;
        }

        protected abstract void Interact(UnitModel unit);
    }
}
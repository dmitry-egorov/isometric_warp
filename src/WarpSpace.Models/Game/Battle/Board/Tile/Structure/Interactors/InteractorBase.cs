using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    public abstract class InteractorBase : IInteractor
    {
        public bool Try_to_Interact_With(UnitModel unit)
        {
            if (!Can_Interact_With(unit))
                return false;

            Interact(unit);
            return true;
        }

        public abstract bool Can_Interact_With(UnitModel unit);
        protected abstract void Interact(UnitModel unit);
    }
}
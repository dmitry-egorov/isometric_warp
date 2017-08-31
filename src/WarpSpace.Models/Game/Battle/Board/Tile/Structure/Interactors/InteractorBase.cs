using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    public abstract class InteractorBase : IInteractor
    {
        private readonly TileModel _tile;

        protected InteractorBase(TileModel tile) => _tile = tile;

        public abstract bool Can_Interact_With(UnitModel unit);

        public bool Try_to_Interact_With(UnitModel unit)
        {
            if (!Is_Adjacent_To(unit) || !Can_Interact_With(unit))
                return false;

            Interact(unit);
            return true;
        }

        protected abstract void Interact(UnitModel unit);

        private bool Is_Adjacent_To(UnitModel unit) => 
            _tile.Is_Adjacent_To(unit.Current_Tile)
        ;
    }
}
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    internal class Empty: InteractorBase
    {
        public override bool Can_Interact_With(UnitModel unit) => false;
        protected override void Interact(UnitModel unit) {}
    }
}
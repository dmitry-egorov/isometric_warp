using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    internal class Empty: InteractorBase
    {
        public Empty(TileModel tile) : base(tile)
        {
        }

        public override bool Can_Interact_With(UnitModel unit)
        {
            return false;
        }

        protected override void Interact(UnitModel unit)
        {
        }
    }
}
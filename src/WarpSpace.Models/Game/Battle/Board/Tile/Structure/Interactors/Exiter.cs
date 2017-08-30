using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    internal class Exiter : InteractorBase
    {
        private readonly GameModel _game;

        public Exiter(GameModel game)
        {
            _game = game;
        }

        public override bool CanBeInteractedBy(UnitModel unit)
        {
            return true;
        }

        protected override void Interact(UnitModel unit)
        {
            _game.RestartBattle();
        }
    }
}
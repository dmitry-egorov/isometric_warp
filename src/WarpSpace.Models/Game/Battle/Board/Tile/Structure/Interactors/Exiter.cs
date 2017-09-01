using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    internal class Exiter : InteractorBase
    {
        private readonly GameModel _game;
        private readonly TileModel _tile;

        public Exiter(GameModel game, TileModel tile)
        {
            _game = game;
            _tile = tile;
        }

        public override bool Can_Interact_With(UnitModel unit) => _tile.Is_Adjacent_To(unit.Current_Tile);
        protected override void Interact(UnitModel unit) => _game.RestartBattle();
    }
}
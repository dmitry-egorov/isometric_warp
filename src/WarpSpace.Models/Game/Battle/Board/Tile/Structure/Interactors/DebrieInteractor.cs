using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    public class DebrieInteractor : InteractorBase
    {
        private readonly TileModel _tile;
        private readonly InventoryContent? _loot;

        public DebrieInteractor(TileModel tile, InventoryContent? loot)
        {
            _tile = tile;
            _loot = loot;
        }

        public override bool Can_Interact_With(UnitModel unit) => _tile.Is_Adjacent_To(unit.Current_Tile);
        protected override void Interact(UnitModel unit)
        {
            unit.Take(_loot);
            _tile.Set_Structure(null);
        }
    }
}
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board.Tile.Structure.Interactors
{
    public interface IInteractor
    {
        bool Can_Interact_With(UnitModel unit);
        bool Try_to_Interact_With(UnitModel unit);
    }
}
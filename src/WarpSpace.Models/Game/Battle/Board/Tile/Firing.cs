using WarpSpace.Models.Game.Battle.Board.Unit;
using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct Firing
    {
        public readonly MWeapon Source;
        public readonly MUnit Target;

        public Firing(MWeapon the_source, MUnit the_target)
        {
            Source = the_source;
            Target = the_target;
        }
    }
}
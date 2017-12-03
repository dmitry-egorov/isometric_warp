using WarpSpace.Models.Game.Battle.Board.Weapon;

namespace WarpSpace.Models.Game.Battle.Board.Tile
{
    public struct Firing
    {
        public readonly MWeapon s_Weapon;
        public readonly MTile s_Target_Tile;

        public Firing(MWeapon the_weapon, MTile the_target_tile)
        {
            s_Weapon = the_weapon;
            s_Target_Tile = the_target_tile;
        }
    }
}